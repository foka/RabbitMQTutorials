# RabbitMQ Tutorials
The repository contains my code to .net tutorials from https://www.rabbitmq.com/getstarted.html.

Read [my notes from tutorials](TutorialsNotes.md).

## Review

* **Who creates a queue - producer or consumer?**

  They both *declare* the queue. Since declaring a queue is idempotent, it doesn't matter who does it first, as long as they both declare it with the same configuration.

* **What happens if they declare the queue with different configurations?**

  The later one will simply fail and get an exception. For example, one party declared a queue with setting `durable: false`, now if the second party tries to declare the same queue with `durable: true`, they get an error: `PRECONDITION_FAILED - inequivalent arg 'durable' for queue 'hello' in vhost '/': received 'true' but current is 'false'`.

* **Producer sends messages to what? To queue, topic, exchange?**

  Lets start from a consumer side: consumer consumes messages from a queue, period. So, effectively messages are delivered to a queue, but they are published to exchanges with a routing information in *routing key*, so you can say: a producer sends messages to an exchange.

* **What is an exchange and a routing key?**

  An exchange is a hub for queues. [It's compared](https://www.rabbitmq.com/tutorials/amqp-concepts.html#amqp-model) to a post office or mailboxes, but I don't fancy this metaphor. It's a hub. You publish a message to an exchange. Exchanges are connected with queues, such connection is called a *binding*. The binding has a routing key and the message has a routing key, so when a message is published, Rabbit matches those routing keys to decide which queues should get a copy of the message.

* **Do I need to provide a routing key always?**

  No, not always. It depends on the type of the exchange. There are four types of exchanges. The tutorials consider three: direct, fanout, and topic. With *fanout* exchange, the routing key is not used (ignored), because when a new message is published to a fanout exchange, a copy of the message is delivered to all queues bound with the exchange.

* **So how the routing key differ between direct and topic exchanges?**

  With a direct exchange, a queue gets the message if their routing keys are equal. With a topic exchange, a routing key is a mask. For example, a message with a routing key set to `quick.orange.rabbit` will be delivered to queues with keys `quick.#` and `*.*.rabbit`.

* **So if the queues in a topic exchange have `#` key, it's like a fanout exchange?**

  Yes.

* **And if I don't use `#` and `*` at all in the queues' keys in a topic exchange, it's like a direct exchange?**

  Yes.

* **What are publisher confirms?**

  If you publish a message you're not 100% sure that the broker actually received the message. Mind, that we're not talking about consumer, just the broker. So if you want to be sure, that Rabbit handled the message, you need to use a confirmation mechanism. This confirmation is called an acknowledgement or ack, and nack for negative acknowledgement.

* **How do I do it?**

  It's all in the benchmarked code of [PublisherConfirms](./PublisherConfirms/). In a nutshell, you either do it synchronously or asynchronously in the publisher. Synchronous mode with waiting for an ack after every message has a significant performance impact but is very easy to implement. On the other hand, asynchronous mode works quick but it requires more code. There's a middle ground, with doing the sync way but in batches.


