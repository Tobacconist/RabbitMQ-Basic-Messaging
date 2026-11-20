# RabbitMQ Messaging System

Two small .NET 10 console applications that show a point-to-point messaging flow over RabbitMQ. The Producer publishes a message to the `hello` queue; the Consumer listens on that queue and prints each message it receives. Built with RabbitMQ.Client 7.x and its async API.

## Project layout

```
Producer/   publishes one message to the "hello" queue and exits
Consumer/   long-running listener, stops on Ctrl+C
```

## Design decisions

- **Manual acknowledgements.** The Consumer sends `BasicAck` only after a message has been processed. If processing throws, it sends `BasicNack` with `requeue: true` so the broker redelivers the message. With `autoAck: true` a crash between delivery and processing would silently lose the message.
- **Durable queue and persistent messages.** The queue is declared with `durable: true` and the Producer sets `DeliveryMode = Persistent`. Both are needed: a durable queue alone still drops non-persistent messages on a broker restart. Producer and Consumer must declare the queue with identical arguments, otherwise RabbitMQ rejects the second declaration with `PRECONDITION_FAILED`.
- **Configurable host.** The broker address comes from the `RABBITMQ_HOST` environment variable and defaults to `localhost`. Credentials are not set in code, so the client uses the default `guest` user, which RabbitMQ only allows from localhost.
- **Failure handling.** If the broker cannot be reached, both applications print a one-line message and exit with code 1 instead of crashing with a stack trace. The Consumer closes its channel and connection cleanly on Ctrl+C.
- **No retry or dead-letter logic.** A message that always fails is requeued indefinitely. That is acceptable for this demo; a real system would cap redeliveries and route poison messages to a dead-letter queue.

## Requirements

- .NET 10 SDK
- A RabbitMQ broker reachable on port 5672 (Docker is the easiest way)

## Running locally

Start RabbitMQ with the management UI:

```powershell
docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:4-management
```

Start the Consumer in one terminal:

```powershell
dotnet run --project Consumer
```

Send messages from another terminal. The message text is an optional argument; without it a default order message is sent:

```powershell
dotnet run --project Producer -- "Order #1043 was cancelled."
```

To use a broker on another machine, set the host first:

```powershell
$env:RABBITMQ_HOST = "rabbit.internal"
```

The management UI is at http://localhost:15672 (user `guest`, password `guest`). It shows queue depth, unacknowledged messages and consumers, which is useful for checking the ack behaviour.

## Troubleshooting

- **`PRECONDITION_FAILED` on startup.** The `hello` queue already exists with different settings (for example a different durability or queue arguments). Both applications print the broker's reason and exit with code 1. Delete the queue in the management UI or with `rabbitmqctl delete_queue hello`, then start again. RabbitMQ 4.x also refuses to declare non-durable queues at all, so an older non-durable version of this code will not run against it.
- **"RabbitMQ'ya bağlanılamadı" (could not connect).** The broker is not running or `RABBITMQ_HOST` points to the wrong address.
- **Messages appear twice.** A message was nacked and requeued after a processing error, or the Consumer exited before acking it. Check the Consumer output for `[!]` lines.
