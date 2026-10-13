import socket
import struct

# Configuración Multicast
MCAST_GRP = '224.1.1.1'
MCAST_PORT = 5000

def iniciar_receptor():
    # Crear el socket UDP
    sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM, socket.IPPROTO_UDP)
    
    # Permitir que varios sockets se conecten a la misma dirección y puerto
    sock.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
    
    # Enlazar al puerto (escuchando en todas las interfaces)
    sock.bind(('', MCAST_PORT))
    
    # Unirse al grupo multicast
    mreq = struct.pack("4sl", socket.inet_aton(MCAST_GRP), socket.INADDR_ANY)
    sock.setsockopt(socket.IPPROTO_IP, socket.IP_ADD_MEMBERSHIP, mreq)
    
    print(f"[NODO] Unido al grupo {MCAST_GRP} en el puerto {MCAST_PORT}. Esperando mensajes...")

    try:
        while True:
            # Esperar datos
            datos, direccion = sock.recvfrom(1024)
            print(f"[NODO] Mensaje recibido: '{datos.decode('utf-8')}' (desde {direccion})")
            
    except KeyboardInterrupt:
        print("\n[NODO] Saliendo del grupo...")
    finally:
        sock.close()

if __name__ == '__main__':
    iniciar_receptor()