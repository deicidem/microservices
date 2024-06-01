using Confluent.Kafka;
using System;
using System.Threading;
using Microsoft.Extensions.Configuration;

class Consumer
{
    static void Main(string[] args)
    {
        const string topic = "MissionCompleted";
        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = "localhost:9092",
            GroupId = "consumer-group-1",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };


        CancellationTokenSource cts = new CancellationTokenSource();

        using (var consumer = new ConsumerBuilder<Ignore, string>(
                   consumerConfig).Build())
        {
            try
            {
                consumer.Subscribe(topic);

                while (true)
                {
                    var cr = consumer.Consume(cts.Token);
                    Console.WriteLine($"Consumed event from topic {topic}: key = {cr.Message.Key} value = {cr.Message.Value}");
                }
            }
            catch (ConsumeException e)
            {
                Console.WriteLine($"Error occured: {e.Error.Reason}");
            }
            catch (OperationCanceledException)
            {

            }
            finally
            {
                consumer.Close();
            }
        }
    }
}