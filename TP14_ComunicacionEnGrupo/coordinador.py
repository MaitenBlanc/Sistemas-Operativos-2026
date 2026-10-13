import socket
import struct
import time

# Configuración Multicast (Para enviar al grupo)
MCAST_GRP = '224.1.1.1'
MCAST_PORT = 5000
MULTICAST_TTL = 2

# Configuración Unicast (Para recibir del exterior)
COORD_PORT = 5001
COORD_IP = '0.0.0.0' # Escucha en todas las interfaces

def iniciar_coordinador():
    # Socket para recibir del exterior (Unicast)
    sock_exterior = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
    sock_exterior.bind((COORD_IP, COORD_PORT))
    print(f"[COORDINADOR] Esperando mensajes externos en el puerto {COORD_PORT}...")

    # Socket para enviar al grupo (Multicast)
    sock_multicast = socket.socket(socket.AF_INET, socket.SOCK_DGRAM, socket.IPPROTO_UDP)
    sock_multicast.setsockopt(socket.IPPROTO_IP, socket.IP_MULTICAST_TTL, MULTICAST_TTL)

    try:
        while True:
            # Recibe el mensaje desde el exterior
            datos, direccion_origen = sock_exterior.recvfrom(1024)
            mensaje_decodificado = datos.decode('utf-8')
            print(f"\n[COORDINADOR] Recibí mensaje del exterior ({direccion_origen}): '{mensaje_decodificado}'")

            # Preparar mensaje para el grupo
            mensaje_a_enviar = f"COORD_DICE:{mensaje_decodificado}".encode('utf-8')

            # Simulación de Atomicidad Básica
            # El coordinador retransmite al grupo
            print(f"[COORDINADOR] Retransmitiendo mensaje al grupo multicast {MCAST_GRP}:{MCAST_PORT}...")
            sock_multicast.sendto(mensaje_a_enviar, (MCAST_GRP, MCAST_PORT))

    except KeyboardInterrupt:
        print("\n[COORDINADOR] Apagando...")
    finally:
        sock_exterior.close()
        sock_multicast.close()

if __name__ == '__main__':
    iniciar_coordinador()