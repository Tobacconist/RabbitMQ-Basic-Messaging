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
                    try
                    {
                        var message = Encoding.UTF8.GetString(ea.Body.Span);
                        Console.WriteLine($" [x] Alınan Mesaj: {message}");

                        await channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($" [!] Mesaj işlenemedi: {ex.Message}");
                        await channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: true);
                    }
                };

                // Ack işlemi mesaj başarıyla işlendikten sonra elle yapılır
                await channel.BasicConsumeAsync(queue: "hello",
                                     autoAck: false,
                                     consumer: consumer);

                Console.WriteLine(" Mesajlar bekleniyor... Çıkmak için [Enter] tuşuna basın.");
                Console.ReadLine();
            }
        }
    }
}