using RabbitMQ.Client;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Producer
{
    class Program
    {
        public static async Task Main()
        {
            // RabbitMQ sunucusuna bağlantı başlatılıyor
            var factory = new ConnectionFactory() { HostName = "localhost" };

            using (var connection = await factory.CreateConnectionAsync())
            using (var channel = await connection.CreateChannelAsync())
            {
                // Kuyruk tanımlama 
                await channel.QueueDeclareAsync(queue: "hello",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                string message = "Hello World! - Bu mesaj güncel v7 Producer'dan geldi.";
                var body = Encoding.UTF8.GetBytes(message);

                // Mesajı gönderme
                await channel.BasicPublishAsync(exchange: "",
                                     routingKey: "hello",
                                     body: body);

                Console.WriteLine($" [x] Gönderilen Mesaj: {message}");
            }

            Console.WriteLine(" Çıkmak için [Enter] tuşuna basın.");
            Console.ReadLine();
        }
    }
}