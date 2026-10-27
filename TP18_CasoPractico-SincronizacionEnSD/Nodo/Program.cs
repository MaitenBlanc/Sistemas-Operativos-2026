using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Nodo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Toma el desfase inicial desde los argumentos de Docker
            int horaLocalSimulada = args.Length > 0 ? int.Parse(args[0]) : 100;
            string hostCoordinador = Environment.GetEnvironmentVariable("COORD_HOST") ?? "localhost";
            
            Console.WriteLine($"--- NODO PARTICIPANTE ---");
            Console.WriteLine($"Hora local inicial: {horaLocalSimulada}s");

            using TcpClient cliente = new TcpClient();
            Console.WriteLine("Conectando al Coordinador...");
            
            // Reintentos de conexión por si el coordinador tarda en levantar en Docker
            while (!cliente.Connected)
            {
                try { await cliente.ConnectAsync(hostCoordinador, 5000); }
                catch { await Task.Delay(1000); }
            }

            NetworkStream stream = cliente.GetStream();
            StreamReader reader = new StreamReader(stream);
            StreamWriter writer = new StreamWriter(stream) { AutoFlush = true };

            while (true)
            {
                string peticion = await reader.ReadLineAsync();
                if (peticion == "TIEMPO")
                {
                    // Enviar hora local
                    await writer.WriteLineAsync(horaLocalSimulada.ToString());
                    
                    // Esperar ajuste del coordinador
                    string ajusteStr = await reader.ReadLineAsync();
                    int ajuste = int.Parse(ajusteStr);
                    
                    // Actualizar reloj simulado
                    horaLocalSimulada += ajuste;
                    Console.WriteLine($"Ajuste recibido del coordinador: {ajusteStr}s");
                    Console.WriteLine($"-> Nueva hora local sincronizada: {horaLocalSimulada}s");
                }
            }
        }
    }
}