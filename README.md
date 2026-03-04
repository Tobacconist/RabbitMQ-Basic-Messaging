#  RabbitMQ Basic Messaging Simulation

> A C# console application project developed to understand and demonstrate the core mechanics of RabbitMQ and message brokering.

This project was built during my internship to study distributed message communication, queue structures, and the decoupling of applications. It demonstrates the fundamental concepts of sending and receiving messages using RabbitMQ's basic implementation.

##  What I Learned & Implemented

* **Core RabbitMQ Elements:** Successfully implemented the **Producer** (sends messages), **Queue** (stores messages based on FIFO logic), and **Consumer** (receives messages) architecture.
* **Exchange Understanding:** Researched how Exchanges direct messages to appropriate queues, specifically learning about `Direct`, `Fanout`, and `Topic` exchange types.
* **Decoupling:** Understood how message brokers allow different services to communicate asynchronously without being directly connected to each other.
* **C# Integration:** Used the `RabbitMQ.Client` NuGet package to establish connections, declare queues, and handle message encoding/decoding (UTF-8).

##  Built With

* C# (.NET Console Application)
* RabbitMQ.Client (NuGet Package)
* RabbitMQ Server (Erlang)

##  How to Run Locally

1. **Prerequisites:** Ensure you have [RabbitMQ Server](https://www.rabbitmq.com/download.html) installed and running locally on your machine (runs on `localhost`).
2. Clone the repository.
3. Open the `.sln` file in Visual Studio.
4. Set the Solution to **Multiple Startup Projects** (Start both `Producer` and `Consumer` simultaneously).
5. Run the application. You will see the Producer sending a "Hello World!" message and the Consumer instantly receiving it from the queue.
