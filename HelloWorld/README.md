# [Hello World](https://www.rabbitmq.com/tutorials/tutorial-one-dotnet.html)

## TIL
* `connection.CreateModel` creates `IModel` which is a session aka channel.
* Declaring a queue is idempotent.
* `EventingBasicConsumer`
* `channel.BasicConsume` consumes all messages and returns.

### Extra
* Counter intuitive defaults at `IModelExensions.QueueDeclare`: `string queue = "", bool durable = false, bool exclusive = true, bool autoDelete = true`, that make a queue very transient.

## To learn in the next tutorials
* `autoAck` in `channel.BasicConsume`
