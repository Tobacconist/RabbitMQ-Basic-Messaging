using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace Producer;

class Program
{
    const string QueueName = "hello";

    public static async Task<int> Main(string[] args)
    {
        var host = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost";
        var factory = new ConnectionFactory { HostName = host };

        var message = args.Length > 0
            ? string.Join(' ', args)
            : "Sipariş #1042 hazırlandı, kargoya verilecek.";

        try
        {
            await using var connection = await factory.CreateConnectionAsync();
            await using var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(queue: QueueName,
                                            durable: true,
                                            exclusive: false,
                                            autoDelete: false);

            var props = new BasicProperties { DeliveryMode = DeliveryModes.Persistent };

            await channel.BasicPublishAsync(exchange: "",
                                            routingKey: QueueName,
                                            mandatory: false,
                                            basicProperties: props,
                                            body: Encoding.UTF8.GetBytes(message));

            Console.WriteLine($" [x] Gönderilen Mesaj: {message}");
        }
        catch (BrokerUnreachableException)
        {
            Console.Error.WriteLine($" RabbitMQ'ya bağlanılamadı ({host}). Sunucunun çalıştığından emin olun.");
            return 1;
        }

        return 0;
    }
}
