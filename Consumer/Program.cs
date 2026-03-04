using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Consumer
{
    class Program
    {
        public static async Task Main()
        {
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

                var consumer = new AsyncEventingBasicConsumer(channel);

                // Mesaj geldiğinde tetiklenecek asenkron olay (Event)
                consumer.ReceivedAsync += async (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine($" [x] Alınan Mesaj: {message}");

                    // Asenkron metodun tamamlanması için
                    await Task.CompletedTask;
                };

                // Kuyruğu dinlemeye başla
                await channel.BasicConsumeAsync(queue: "hello",
                                     autoAck: true,
                                     consumer: consumer);

                Console.WriteLine(" Mesajlar bekleniyor... Çıkmak için [Enter] tuşuna basın.");
                Console.ReadLine();
            }
        }
    }
}