using System.Collections.Concurrent;
using System.Text;
using RabbitMQ.Client;

var factory = new ConnectionFactory() { HostName = "localhost" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.ExchangeDeclare(exchange: "publisher_confirms_tutorial", type: ExchangeType.Topic);
channel.ConfirmSelect();

var messageCount = 1000;
var messageBody = Encoding.UTF8.GetBytes("Lorem ipsum dolor sit amet enim.");
var basicProperties = channel.CreateBasicProperties();

PublishMessagesIndividually();
PublishMessagesInBatches(batchSize: 200);
await HandlePublisherConfirmsAsynchronously();


void PublishMessagesIndividually()
{
    for (int i = 0; i < messageCount; i++)
    {
        channel.BasicPublish(exchange: "publisher_confirms_tutorial", routingKey: "", basicProperties, messageBody);
        channel.WaitForConfirmsOrDie(TimeSpan.FromSeconds(5));
    }
}

void PublishMessagesInBatches(int batchSize)
{
    int i = 0;
    while (i < messageCount)
    {
        channel.BasicPublish(exchange: "publisher_confirms_tutorial", routingKey: "", basicProperties, messageBody);

        i++;
        if (i % batchSize == 0)
        {
            channel.WaitForConfirmsOrDie(TimeSpan.FromSeconds(5));
        }
    }

    var hasUnconfirmedMessages = i % batchSize != 0;
    if (hasUnconfirmedMessages)
    {
        channel.WaitForConfirmsOrDie(TimeSpan.FromSeconds(5));
    }
}

async Task HandlePublisherConfirmsAsynchronously()
{
    var outstandingConfirms = new ConcurrentDictionary<ulong, string>();

    channel.BasicAcks += (sender, ea) => cleanOutstandingConfirms(ea.DeliveryTag, ea.Multiple);
    channel.BasicNacks += (sender, ea) =>
    {
        outstandingConfirms.TryGetValue(ea.DeliveryTag, out string body);
        Console.WriteLine($"Message with body {body} has been nack-ed. Sequence number: {ea.DeliveryTag}, multiple: {ea.Multiple}");
        cleanOutstandingConfirms(ea.DeliveryTag, ea.Multiple);
    };

    for (int i = 0; i < messageCount; i++)
    {
        channel.BasicPublish(exchange: "publisher_confirms_tutorial", routingKey: "", basicProperties, messageBody);
    }

    await Task.Run(() =>
    {
        while (outstandingConfirms.Count != 0)
            Task.Delay(100);
    });

    void cleanOutstandingConfirms(ulong sequenceNumber, bool multiple)
    {
        if (multiple)
        {
            var confirmed = outstandingConfirms.Where(k => k.Key <= sequenceNumber);
            foreach (var entry in confirmed)
            {
                outstandingConfirms.TryRemove(entry.Key, out _);
            }
        }
        else
        {
            outstandingConfirms.TryRemove(sequenceNumber, out _);
        }
    }
}
