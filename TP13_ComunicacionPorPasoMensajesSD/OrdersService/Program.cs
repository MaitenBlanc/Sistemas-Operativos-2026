using System;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;

class Program
{
    static async Task Main(string[] args)
    {
        // Configurar la conexión a RabbitMQ
        var factory = new ConnectionFactory() { HostName = "localhost" };
        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        // Declarar la cola
        await channel.QueueDeclareAsync(queue: "pedidos_queue",
                                        durable: false,
                                        exclusive: false,
                                        autoDelete: false,
                                        arguments: null);

        // Crear el mensaje
        string message = "Pedido Creado: Orden #12345 - Usuario: juan@email.com";
        var body = Encoding.UTF8.GetBytes(message);

        // Publicar el mensaje en la cola
        await channel.BasicPublishAsync(exchange: "",
                                        routingKey: "pedidos_queue",
                                        body: body);

        Console.WriteLine($"[x] Enviado al broker: {message}");
    }
}