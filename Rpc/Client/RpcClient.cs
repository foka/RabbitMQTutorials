using System.Collections.Concurrent;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

public class RpcClient : IDisposable
{
    private readonly IConnection connection;
    private readonly IModel channel;
    private readonly BlockingCollection<string> replyQueue = new BlockingCollection<string>();
    private readonly IBasicProperties props;

    public RpcClient()
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };

        this.connection = factory.CreateConnection();
        this.channel = connection.CreateModel();
        var replyQueueName = channel.QueueDeclare().QueueName;
        var consumer = new EventingBasicConsumer(channel);

        this.props = channel.CreateBasicProperties();
        var correlationId = Guid.NewGuid().ToString();
        this.props.CorrelationId = correlationId;
        this.props.ReplyTo = replyQueueName;

        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var reply = Encoding.UTF8.GetString(body);
            if (ea.BasicProperties.CorrelationId == correlationId)
            {
                replyQueue.Add(reply);
            }
        };

        this.channel.BasicConsume(
            consumer: consumer,
            queue: replyQueueName,
            autoAck: true);
    }

    public string Call(string message)
    {
        var messageBytes = Encoding.UTF8.GetBytes(message);
        channel.BasicPublish(
            exchange: "",
            routingKey: "rpc_queue",
            basicProperties: this.props,
            body: messageBytes);

        return replyQueue.Take();
    }

    public void Dispose()
    {
        this.channel.Dispose();
        this.connection.Dispose();
    }
}
