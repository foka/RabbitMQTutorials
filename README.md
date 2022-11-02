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
* Work queue as a [Competing Consumers pattern](https://www.enterpriseintegrationpatterns.com/patterns/messaging/CompetingConsumers.html).
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

---

## [Publish/Subscribe](https://www.rabbitmq.com/tutorials/tutorial-three-dotnet.html)
* [Code](./PublishSubscribe)

### TIL
* `Exchange`
* Exchange types (fanout)
* Exchange-queue routing key

### To learn
* Exchange types: direct, topic, headers and fanout
* ⌛ [Publisher Confirms](https://www.rabbitmq.com/confirms.html)
* ⌛ For more information on IModel methods and IBasicProperties, you can browse the [RabbitMQ .NET client API reference online](https://rabbitmq.github.io/rabbitmq-dotnet-client/api/RabbitMQ.Client.html).


---

## [Routing](https://www.rabbitmq.com/tutorials/tutorial-four-dotnet.html)
* [Code](./Routing)

### TIL
* Binding (of queue and exchange)
* `fanout` exchange ignores `binding key`
* `direct` exchange type: a message goes to the queues whose binding key exactly matches the routing key of the message

### To learn
* ⌛ Exchange types: topic, headers
* ⌛ [Publisher Confirms](https://www.rabbitmq.com/confirms.html)
* ⌛ For more information on IModel methods and IBasicProperties, you can browse the [RabbitMQ .NET client API reference online](https://rabbitmq.github.io/rabbitmq-dotnet-client/api/RabbitMQ.Client.html).

---

## [Topics](https://www.rabbitmq.com/tutorials/tutorial-five-dotnet.html)
* [Code](./Topics)

### TIL
* `topic` exchange type
* `*` (star) can substitute for exactly one word.
* `#` (hash) can substitute for zero or more words.
* There can be as many words in the routing key as you like, up to the limit of 255 bytes.

### To learn
* ⌛ Exchange types: headers
* ⌛ [Publisher Confirms](https://www.rabbitmq.com/confirms.html)
* ⌛ For more information on IModel methods and IBasicProperties, you can browse the [RabbitMQ .NET client API reference online](https://rabbitmq.github.io/rabbitmq-dotnet-client/api/RabbitMQ.Client.html).

---

## [RPC](https://www.rabbitmq.com/tutorials/tutorial-six-dotnet.html)
* [Code](./Rpc)

### TIL
* `BlockingCollection<T>`

### To learn
* ⌛ Exchange types: headers
* ⌛ [Publisher Confirms](https://www.rabbitmq.com/confirms.html)
* ⌛ For more information on IModel methods and IBasicProperties, you can browse the [RabbitMQ .NET client API reference online](https://rabbitmq.github.io/rabbitmq-dotnet-client/api/RabbitMQ.Client.html).