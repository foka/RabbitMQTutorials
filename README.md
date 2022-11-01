# [RabbitMQ Tutorials](https://www.rabbitmq.com/getstarted.html)

## [Hello World](https://www.rabbitmq.com/tutorials/tutorial-one-dotnet.html)
* [Code](./HelloWorld)

### TIL
* `connection.CreateModel` creates `IModel` which is a session aka channel.
* Declaring a queue is idempotent.
* `EventingBasicConsumer`
* `channel.BasicConsume` consumes all messages and returns.

#### Extra
* Counter intuitive defaults at `IModelExensions.QueueDeclare`: `string queue = "", bool durable = false, bool exclusive = true, bool autoDelete = true`, that make a queue very transient.

### To learn in the next tutorials
* `autoAck` in `channel.BasicConsume`

---
## [Work Queues](https://www.rabbitmq.com/tutorials/tutorial-two-dotnet.html)
* [Code](./WorkQueues)

### TIL
* "Work queue" as a pattern.
* Message ack(nowledgement) - consumer tells RabbitMQ that a particular message has been received, processed and that RabbitMQ is free to delete it.
  * Default timeout on ack delivery = 30 minutes.
  * `autoAck` in `channel.BasicConsume`
  * Acknowledgement must be sent on the same channel that received the delivery.
* RabbitMQ doesn't allow you to redefine an existing queue with different parameters.
* durable queue and messages
* BasicQos method with the prefetchCount = 1 (*don't dispatch a new message to a worker until it has processed and acknowledged the previous one*).

### To learn
* [Publisher Confirms](https://www.rabbitmq.com/confirms.html)
* For more information on IModel methods and IBasicProperties, you can browse the [RabbitMQ .NET client API reference online](https://rabbitmq.github.io/rabbitmq-dotnet-client/api/RabbitMQ.Client.html).