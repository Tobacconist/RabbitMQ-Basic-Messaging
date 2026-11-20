# RabbitMQ Messaging System

Producer, `hello` kuyruğuna mesaj yazar; Consumer aynı kuyruğu dinleyip mesajları ekrana basar. .NET 10 ve RabbitMQ.Client 7.x ile yazılmış iki küçük konsol uygulamasıdır.

## Teknik tercihler

- **Manuel ack**: Consumer mesajı işledikten sonra `BasicAck` gönderir. İşleme sırasında hata olursa `BasicNack` ile mesaj kuyruğa geri döner. `autoAck` kullanılsaydı process çöktüğünde mesaj kaybolurdu.
- **Durable kuyruk, persistent mesaj**: RabbitMQ yeniden başlasa da kuyruk ve içindeki mesajlar diskten geri yüklenir. Kuyruk ayarları iki tarafta aynı olmak zorundadır, aksi halde `PRECONDITION_FAILED` hatası alınır.
- **Host ayarı**: Sunucu adresi `RABBITMQ_HOST` ortam değişkeninden okunur, tanımlı değilse `localhost` kullanılır.
- **Hata yönetimi**: Sunucuya ulaşılamazsa iki uygulama da anlaşılır bir mesajla çıkış kodu 1 döner. Consumer Ctrl+C ile bağlantıyı düzgün kapatır.

## Yerelde çalıştırma

RabbitMQ'yu başlatın:

```powershell
docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:4-management
```

Consumer'ı bir terminalde çalıştırın:

```powershell
dotnet run --project Consumer
```

Başka bir terminalde mesaj gönderin (mesaj argümanı opsiyoneldir):

```powershell
dotnet run --project Producer -- "Sipariş #1043 iptal edildi."
```

Yönetim arayüzü: http://localhost:15672 (kullanıcı `guest`, şifre `guest`).

Eğer `hello` kuyruğu daha önce `durable: false` ile oluşturulduysa, arayüzden silip yeniden çalıştırın.
