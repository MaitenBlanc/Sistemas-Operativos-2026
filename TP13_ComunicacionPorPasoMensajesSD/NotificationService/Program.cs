using System;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

class Program
{
    static async Task Main(string[] args)
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(queue: "pedidos_queue",
                                        durable: false,
                                        exclusive: false,
                                        autoDelete: false,
                                        arguments: null);

        Console.WriteLine("[*] Esperando mensajes de pedidos. Presiona [Enter] para salir.");

        // Crear el consumidor asíncrono
        var consumer = new AsyncEventingBasicConsumer(channel);
        
        // Definir qué hacer cuando llegue un mensaje (evento ReceivedAsync)
        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine($"[x] Recibido de RabbitMQ: {message}");
            Console.WriteLine($"[ ] Simulando envío de correo al cliente...");
            
            await Task.Yield(); // Permite que el hilo continúe si hubiera operaciones pesadas
        };

        // Iniciar el consumo de la cola de forma asíncrona
        await channel.BasicConsumeAsync(queue: "pedidos_queue",
                                        autoAck: true,
                                        consumer: consumer);

        Console.ReadLine();
    }
}