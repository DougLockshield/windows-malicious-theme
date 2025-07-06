from flask import Flask, request  # Importa o framework Flask e o objeto de requisição HTTP
import datetime, os              # Importa módulos para manipular datas e o sistema de arquivos

app = Flask(__name__)            # Inicializa o app Flask

# Define a rota '/loot' que aceita apenas requisições POST
@app.route('/loot', methods=['POST'])
def loot():
    raw = request.get_data()  # Obtém os dados brutos do corpo da requisição (sem decodificação)

    # Gera um timestamp no formato 'YYYYMMDD_HHMMSS' para criar nomes únicos de arquivos
    ts = datetime.datetime.now().strftime('%Y%m%d_%H%M%S')

    # Define o nome do arquivo de saída com base no timestamp
    fname = f"userdata_{ts}.tar"

    # Abre o arquivo no modo binário para escrita e salva os dados recebidos
    with open(fname, 'wb') as f:
        f.write(raw)

    # Calcula o tamanho do arquivo salvo em megabytes
    size = os.path.getsize(fname) / 1024 / 1024

    # Exibe no terminal que um novo arquivo foi recebido, com o nome e o tamanho
    print(f"[+] Recebido {fname} ({size:.2f} MB)")

    # Retorna uma resposta de sucesso HTTP 200 para o cliente
    return 'OK', 200

# Inicia o servidor Flask escutando todas as interfaces de rede na porta 5000
if __name__ == '__main__':
    app.run(host='0.0.0.0', port=5000)
