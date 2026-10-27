using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Coordinador
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("--- Iniciando NODO COORDINADOR (Berkeley) ---");
            int puerto = 5000;
            TcpListener listener = new TcpListener(IPAddress.Any, puerto);
            listener.Start();
            
            List<TcpClient> clientes = new List<TcpClient>();
            Console.WriteLine($"Esperando a los 3 nodos participantes en el puerto {puerto}...");

            while (clientes.Count < 3)
            {
                TcpClient cliente = await listener.AcceptTcpClientAsync();
                clientes.Add(cliente);
                Console.WriteLine($"Nodo conectado. ({clientes.Count}/3)");
            }

           while (true)
            {
                // Simula la hora del coordinador (inicia en 100 en cada iteración)
                int horaCoordinador = 100; 
                List<int> diferencias = new List<int> { 0 }; // La diferencia del coordinador consigo mismo es 0
                bool falloDetectado = false; // Bandera para saber si se desconectó alguien

                // Solicitar hora actual a cada nodo y obtener diferencias
                foreach (var cliente in clientes)
                {
                    NetworkStream stream = cliente.GetStream();
                    StreamReader reader = new StreamReader(stream);
                    StreamWriter writer = new StreamWriter(stream) { AutoFlush = true };

                    await writer.WriteLineAsync("TIEMPO");
                    string respuesta = await reader.ReadLineAsync();
                    
                    // Comprobar si la respuesta es null (desconexión)
                    if (respuesta == null) 
                    {
                        Console.WriteLine($"\n[ERROR] Un nodo se ha desconectado o no responde. Abortando sincronización.");
                        falloDetectado = true;
                        break; // Rompe el foreach actual de sincronización
                    }

                    int horaNodo = int.Parse(respuesta);
                    
                    int diferencia = horaNodo - horaCoordinador;
                    diferencias.Add(diferencia);
                    Console.WriteLine($"Hora del nodo recibida: {horaNodo} (Diferencia: {diferencia}s)");
                }

                // Si se detectó un fallo, sale del bucle principal
                if (falloDetectado)
                {
                    break;
                }

                // Calcular promedio de las diferencias
                int promedioDiferencias = (int)diferencias.Average();
                Console.WriteLine($"\n-> Diferencia promedio calculada: {promedioDiferencias}s");

                // Ajusta su propio reloj simulado
                horaCoordinador += promedioDiferencias;
                Console.WriteLine($"-> Hora del Coordinador ajustada a: {horaCoordinador}s\n");

                // Determinar y enviar el ajuste correspondiente a cada servidor
                for (int i = 0; i < clientes.Count; i++)
                {
                    int ajuste = promedioDiferencias - diferencias[i + 1];
                    NetworkStream stream = clientes[i].GetStream();
                    StreamWriter writer = new StreamWriter(stream) { AutoFlush = true };
                    
                    Console.WriteLine($"Enviando ajuste de {ajuste}s al Nodo {i + 1}...");
                    await writer.WriteLineAsync(ajuste.ToString());
                }

                Console.WriteLine("\nSincronización completada. Esperando para la próxima iteración...");
                Thread.Sleep(5000); // Intervalo configurable antes de repetir
            }
            
            Console.WriteLine("El coordinador se ha detenido debido a una falla en la red de nodos.");
        }
    }
}