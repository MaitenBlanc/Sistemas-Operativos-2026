import socket

# IP del Coordinador ('127.0.0.1' si se prueba en la misma PC o la IP local de la PC que corre coordinador.py)
IP_COORDINADOR = '127.0.0.1' 
PUERTO_COORDINADOR = 5001

def enviar_mensaje_exterior():
    sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
    
    mensaje = input("Escriba el mensaje a enviar desde el exterior: ")
    
    print(f"[EXTERIOR] Enviando al coordinador en {IP_COORDINADOR}:{PUERTO_COORDINADOR}...")
    sock.sendto(mensaje.encode('utf-8'), (IP_COORDINADOR, PUERTO_COORDINADOR))
    
    print("[EXTERIOR] Mensaje enviado.")
    sock.close()

if __name__ == '__main__':
    enviar_mensaje_exterior()