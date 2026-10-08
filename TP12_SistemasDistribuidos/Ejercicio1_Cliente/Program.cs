using System;
using System.Net.Sockets;
using System.Text;

class TcpClientExample
{
    static void Main()
    {
        try
        {
            int port = 13000;
            // (CONNECT) Establece conexión activa con el servidor
            TcpClient client = new TcpClient("127.0.0.1", port);
            
            string message = "Hola, esta es una comunicación confiable por stream.";
            byte[] data = Encoding.UTF8.GetBytes(message);

            NetworkStream stream = client.GetStream();
            
            // (SEND) Envía datos
            stream.Write(data, 0, data.Length);
            Console.WriteLine($"Enviado: {message}");

            // (RECEIVE) Lee la respuesta del servidor
            data = new byte[256];
            int bytes = stream.Read(data, 0, data.Length);
            string responseData = Encoding.UTF8.GetString(data, 0, bytes);
            Console.WriteLine($"Respuesta del servidor: {responseData}");

            // (CLOSE) Libera el socket
            stream.Close();
            client.Close();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}