import pika
import json
import time
import random
from datetime import datetime

RABBITMQ_HOST = 'localhost'
QUEUE_NAME = 'radares'

def conectar_rabbitmq():
    """Conecta ao RabbitMQ com retentativa (Resiliência)"""
    credentials = pika.PlainCredentials('admin', 'admin123')
    parameters = pika.ConnectionParameters(RABBITMQ_HOST, 5672, '/', credentials)
    
    try:
        connection = pika.BlockingConnection(parameters)
        channel = connection.channel()
        # Cria a fila se ela não existir (Durable = não perde mensagens se cair a luz)
        channel.queue_declare(queue=QUEUE_NAME, durable=True)
        print(f"[*] Conectado ao RabbitMQ na fila '{QUEUE_NAME}'")
        return connection, channel
    except Exception as e:
        print(f"[!] Erro ao conectar: {e}. Tentando em 5s...")
        time.sleep(5)
        return conectar_rabbitmq()

def gerar_dados_radar():
    """Simula a leitura de um sensor físico"""
    placas_letras = ['ABC', 'DEF', 'GHI', 'JKL', 'MNO', 'PQR']
    
    dados = {
        "sensor_id": f"RADAR-{random.randint(100, 999)}",
        "data_hora": datetime.now().isoformat(),
        # Gera uma placa aleatória tipo ABC-1234
        "placa": f"{random.choice(placas_letras)}-{random.randint(1000, 9999)}",
        # Velocidade simulada: maioria normal, alguns apressadinhos
        "velocidade": random.choices([40, 60, 55, 65, 90, 110], weights=[20, 40, 20, 10, 5, 5])[0],
        "via": "Av. Sampaio Vidal"
    }
    return dados

def iniciar_simulacao():
    connection, channel = conectar_rabbitmq()

    print(" [*] Radar ligado. Pressione CTRL+C para parar.")
    
    try:
        while True:
            # 1. Gera o dado
            infracao = gerar_dados_radar()
            mensagem_json = json.dumps(infracao)
            
            # 2. Publica na fila
            channel.basic_publish(
                exchange='',
                routing_key=QUEUE_NAME,
                body=mensagem_json,
                properties=pika.BasicProperties(
                    delivery_mode=2,  # Mensagem persistente (salva em disco)
                )
            )
            
            print(f"[x] Enviado: {infracao['placa']} | {infracao['velocidade']} km/h")
            
            # Simula o tempo entre um carro e outro passar (0.5 a 2 segundos)
            time.sleep(random.uniform(0.5, 2.0))
            
    except KeyboardInterrupt:
        print("\nDesligando radar...")
        connection.close()

iniciar_simulacao()