#include <iostream>
#include <string>
#include <algorithm>
#include <sys/socket.h>
#include <arpa/inet.h>
#include <unistd.h>
#include <cstring>

using namespace std;

int main(int argc, char* argv[]) {
    // Validar que se pase el ID del proceso al ejecutar
    if (argc != 2) {
        cerr << "Uso: " << argv[0] << " <ID_Proceso (1 al 4)>" << endl;
        return 1;
    }

    int id = stoi(argv[1]);
    int lamport_clock = 0; // Estado inicial del reloj de Lamport
    int port = 5000 + id;  // Puertos: P1=5001, P2=5002, P3=5003, P4=5004

    // Configuración del socket UDP para recibir mensajes
    int sock = socket(AF_INET, SOCK_DGRAM, 0);
    sockaddr_in server_addr{};
    server_addr.sin_family = AF_INET;
    server_addr.sin_port = htons(port);
    server_addr.sin_addr.s_addr = INADDR_ANY;

    bind(sock, (struct sockaddr*)&server_addr, sizeof(server_addr));

    cout << "--- INICIANDO COMPUTADORA " << id << " ---" << endl;

    if (id == 1) {
        // Proceso 1 inicia la cadena
        lamport_clock++; // Incrementa su reloj antes de su evento local
        cout << "[P" << id << "] Ejecutando aplicacion. Reloj de Lamport = " << lamport_clock << endl;
        
        // Simular trabajo
        sleep(2);

        // Preparar envío al Proceso 2
        lamport_clock++; // Incrementa antes de enviar el mensaje
        int next_port = 5000 + id + 1;
        sockaddr_in next_addr{};
        next_addr.sin_family = AF_INET;
        next_addr.sin_port = htons(next_port);
        inet_pton(AF_INET, "127.0.0.1", &next_addr.sin_addr);
        
        sendto(sock, &lamport_clock, sizeof(lamport_clock), 0, (struct sockaddr*)&next_addr, sizeof(next_addr));
        cout << "[P" << id << "] Mensaje enviado a P" << id + 1 << " con marca de tiempo: " << lamport_clock << endl;

    } else {
        // Procesos 2, 3 y 4 deben esperar el mensaje del proceso anterior para sincronizarse
        cout << "[P" << id << "] Esperando turno para ejecutar..." << endl;
        
        int received_clock;
        sockaddr_in client_addr{};
        socklen_t client_len = sizeof(client_addr);
        
        // Bloquea hasta recibir el mensaje
        recvfrom(sock, &received_clock, sizeof(received_clock), 0, (struct sockaddr*)&client_addr, &client_len);
        
        // ALGORITMO DE LAMPORT: max(local, recibido) + 1
        lamport_clock = max(lamport_clock, received_clock) + 1;
        
        cout << "[P" << id << "] Mensaje recibido. Reloj de Lamport sincronizado a = " << lamport_clock << endl;
        cout << "[P" << id << "] Ejecutando aplicacion." << endl;
        
        // Simular trabajo
        sleep(2);
        
        // Si no es el último proceso, le avisa al siguiente
        if (id < 4) {
            lamport_clock++; // Incrementa antes de enviar
            int next_port = 5000 + id + 1;
            sockaddr_in next_addr{};
            next_addr.sin_family = AF_INET;
            next_addr.sin_port = htons(next_port);
            inet_pton(AF_INET, "127.0.0.1", &next_addr.sin_addr);
            
            sendto(sock, &lamport_clock, sizeof(lamport_clock), 0, (struct sockaddr*)&next_addr, sizeof(next_addr));
            cout << "[P" << id << "] Mensaje enviado a P" << id + 1 << " con marca de tiempo: " << lamport_clock << endl;
        } else {
            cout << "[P4] Ejecucion finalizada. Fin de la cadena de sincronizacion." << endl;
        }
    }

    close(sock);
    return 0;
}