# [Worker Queues](https://www.rabbitmq.com/tutorials/tutorial-two-dotnet.html)

## TIL
* "Worker queue" as a pattern.
* Message ack(nowledgement) - consumer tells RabbitMQ that a particular message has been received, processed and that RabbitMQ is free to delete it.
  * Default timeout on ack delivery = 30 minutes
  * `autoAck` in `channel.BasicConsume`
  * Acknowledgement must be sent on the same channel that received the delivery
* RabbitMQ doesn't allow you to redefine an existing queue with different parameters

### Extra

## To learn
* [Publisher Confirms](https://www.rabbitmq.com/confirms.html)