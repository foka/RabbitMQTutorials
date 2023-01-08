using System.Collections.Concurrent;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using RabbitMQ.Client;

public class Program
{
    private static readonly long messageCount = 50000;
    private static readonly byte[] messageBody = Encoding.UTF8.GetBytes("Lorem ipsum dolor sit amet enim.");

    public static void Main(string[] args)
    {
        var summary = BenchmarkRunner.Run<Program>(
            DefaultConfig.Instance.AddJob(
                Job.Default.WithWarmupCount(1)
                    .WithIterationCount(1)
                    .WithUnrollFactor(1)
            )
        );
        Console.WriteLine(summary.ToString());
    }

    [Benchmark]
    public void PublishMessagesIndividually()
    {
        using var connection = CreateConnection();
        using var channel = CreateChannel(connection);
        var basicProperties = channel.CreateBasicProperties();

        for (int i = 0; i < messageCount; i++)
        {
            channel.BasicPublish(exchange: "publisher_confirms_tutorial", routingKey: "", basicProperties, messageBody);
            channel.WaitForConfirmsOrDie(TimeSpan.FromSeconds(5));
        }
    }

    [Benchmark]
    public void PublishMessagesInBatches100()
    {
        int batchSize = 100;
        using var connection = CreateConnection();
        using var channel = CreateChannel(connection);
        var basicProperties = channel.CreateBasicProperties();

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

    [Benchmark]
    public void HandlePublisherConfirmsAsynchronously()
    {
        using var connection = CreateConnection();
        using var channel = CreateChannel(connection);
        var basicProperties = channel.CreateBasicProperties();

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

        while (outstandingConfirms.Count != 0)
            Thread.Sleep(1);

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


    private static IConnection CreateConnection()
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        var connection = factory.CreateConnection();
        return connection;
    }

    private static IModel CreateChannel(IConnection connection)
    {
        var channel = connection.CreateModel();
        channel.ExchangeDeclare(exchange: "publisher_confirms_tutorial", type: ExchangeType.Topic);
        channel.ConfirmSelect();
        return channel;
    }
}