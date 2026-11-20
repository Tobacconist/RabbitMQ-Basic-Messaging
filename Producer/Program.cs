using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Producer
{
    class Program
    {
        const string QueueName = "hello";

        public static async Task<int> Main()
        {
            var host = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost";
            var factory = new ConnectionFactory() { HostName = host };

            try
            {
                using (var connection = await factory.CreateConnectionAsync())
                using (var channel = await connection.CreateChannelAsync())
                {
                    await channel.QueueDeclareAsync(queue: QueueName,
                                         durable: true,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);

                    string message = "Hello World! - Bu mesaj güncel v7 Producer'dan geldi.";
                    var body = Encoding.UTF8.GetBytes(message);

                    var props = new BasicProperties { DeliveryMode = DeliveryModes.Persistent };

                    await channel.BasicPublishAsync(exchange: "",
                                         routingKey: QueueName,
                                         mandatory: false,
                                         basicProperties: props,
                                         body: body);

                    Console.WriteLine($" [x] Gönderilen Mesaj: {message}");
                }
            }
            catch (BrokerUnreachableException)
            {
                Console.Error.WriteLine($" RabbitMQ'ya bağlanılamadı ({host}). Sunucunun çalıştığından emin olun.");
                return 1;
            }

            return 0;
        }
    }
}
