# 🎭 Windows Malicious Theme - Proof of Concept (PoC)

> ⚠️ **Este projeto é apenas para fins educacionais.** Ele demonstra como arquivos de tema do Windows podem ser utilizados como vetor de execução de código, comprometendo a segurança do usuário. **Não use para fins maliciosos.**

---

## 🧠 Visão Geral

Essa PoC mostra como um arquivo `.theme` aparentemente inofensivo pode ativar um protetor de tela (.scr) – que, na verdade, é um executável .NET disfarçado. O `.scr` gera e executa um script PowerShell que copia a pasta *User Data* do Chrome, compacta em TAR e envia para um servidor Flask.

---

## 🎯 Objetivo

- Demonstrar o risco de compartilhar arquivos `.theme` / `.deskthemepack`.
- Simular um roubo de dados usando engenharia social.
- Conscientizar sobre personalizações baixadas de fontes desconhecidas.

---

## 📂 Estrutura dos Arquivos

```text
/
├── Malycius.theme           # Arquivo de tema malicioso
├── Darktheme.scr            # Executável (.NET) renomeado como .scr
├── wallpaper.jpg            # Imagem usada como plano de fundo
├── Darktheme.cs             # Código‑fonte do executável (.scr)
└── server.py                # Listener Flask (recebe o .tar)
```

---

## ⚙️ Como Funciona

1. **`Malycius.theme`** define `wallpaper.jpg` como fundo e referencia `Darktheme.scr` como protetor de tela.
2. Ao aplicar o tema, o Windows registra `Darktheme.scr` no registro (`SCRNSAVE.EXE`).
3. Quando o protetor de tela dispara (timeout = 1 s), `Darktheme.scr`:
   - Cria `send_chrome_userdata.ps1` no `%TEMP%`.
   - O PowerShell copia `User Data` do Chrome com *robocopy*, gera `chrome_userdata.tar` e envia via HTTP POST para `/loot` no **server.py**.
4. **`server.py`** grava o arquivo recebido com timestamp.

---

## 💻 Requisitos

- .NET 8.0 SDK – compilar **Darktheme.cs → Darktheme.scr**
- Python 3 + Flask → `pip install flask`
- Windows 10/11 (tar.exe)

---

## 🚀 Passo a Passo (ambiente isolado)

### 1. Compilar o protetor de tela
```bash
# No diretório do código
csc -out:Darktheme.exe Darktheme.cs
ren Darktheme.exe Darktheme.scr
```

### 2. Iniciar o listener Flask
```bash
python server.py
```
(Saída esperada depois de um envio: `[+] Recebido userdata_20250706_114455.tar (xx MB)`)

### 3. Montar o pacote e aplicar o tema
```text
Malycius.theme
Darktheme.scr
wallpaper.jpg
```
1. Selecione os três arquivos → *Enviar para ▸ Pasta compactada (.zip)*.
2. Renomeie o zip para **Malycius.deskthemepack** ou gere `Malycius.themepack` via `makecab`.
3. Clique duas vezes → o wallpaper muda; após ~1 s de inatividade, o payload roda e o tar aparece no servidor.

---
### 🎬 Vídeo demonstrativo
https://youtu.be/Sc5INTLvD0M

## 🛡️ Aviso Legal
Uso estritamente didático. Não execute em sistemas de terceiros sem autorização.

---

## ✨ Contribuições
Issues e PRs sobre bypasses de EDR, outros navegadores ou variações de tema são bem‑vindos.
