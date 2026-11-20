using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Consumer
{
    class Program
    {
        const string QueueName = "hello";

        public static async Task<int> Main()
        {
            var host = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost";
            var factory = new ConnectionFactory() { HostName = host };

            using var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (_, e) =>
            {
                e.Cancel = true;
                cts.Cancel();
            };

            try
            {
                using (var connection = await factory.CreateConnectionAsync(cts.Token))
                using (var channel = await connection.CreateChannelAsync(cancellationToken: cts.Token))
                {
                    await channel.QueueDeclareAsync(queue: QueueName,
                                         durable: true,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null,
                                         cancellationToken: cts.Token);

                    var consumer = new AsyncEventingBasicConsumer(channel);

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
                    await channel.BasicConsumeAsync(queue: QueueName,
                                         autoAck: false,
                                         consumer: consumer,
                                         cancellationToken: cts.Token);

                    Console.WriteLine(" Mesajlar bekleniyor... Çıkmak için Ctrl+C.");
                    await Task.Delay(Timeout.Infinite, cts.Token);
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine(" Kapatılıyor.");
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
