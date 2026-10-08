using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

class TcpServer
{
    static void Main()
    {
        TcpListener server = null;
        try
        {
            int port = 13000;
            server = new TcpListener(IPAddress.Parse("127.0.0.1"), port);
            server.Start(); // Inicia la espera de conexiones (LISTEN)

            Console.WriteLine("Esperando una conexión TCP...");
            TcpClient client = server.AcceptTcpClient(); // (ACCEPT) Bloquea hasta que llega un cliente
            Console.WriteLine("¡Cliente conectado!");

            NetworkStream stream = client.GetStream();
            byte[] bytes = new byte[256];
            int i;

            // (RECEIVE) Lee el stream continuo
            while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
            {
                string data = Encoding.UTF8.GetString(bytes, 0, i);
                Console.WriteLine($"Recibido: {data}");
                
                // (SEND) Devuelve una respuesta confiable
                byte[] msg = Encoding.UTF8.GetBytes("Mensaje recibido correctamente");
                stream.Write(msg, 0, msg.Length);
            }
            client.Close();
        }
        catch (SocketException e)
        {
            Console.WriteLine(e);
        }
        finally
        {
            server?.Stop();
        }
    }
}