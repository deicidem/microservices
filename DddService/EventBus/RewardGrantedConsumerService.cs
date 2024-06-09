using System.Text.Json;
using Confluent.Kafka;
using DddService.Features.CommandCenterFeature;
using MediatR;
using Messages;

namespace DddService.EventBus;

public class RewardGrantedConsumerService : IHostedService
{
    private readonly IConfiguration _configuration;
    private readonly IMediator _mediator;
    private readonly IConsumer<Null, string> _consumer;


    public RewardGrantedConsumerService(IConfiguration configuration, IMediator mediator)
    {
        _configuration = configuration;
        _mediator = mediator;

        var consumerconfig = new ConsumerConfig
        {
            BootstrapServers = _configuration["Kafka:BootstrapServers"],
            GroupId = "consumer-group-1"
        };

        _consumer = new ConsumerBuilder<Null, string>(consumerconfig).Build();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _consumer.Subscribe("RewardGranted");

        Task.Run(async () =>
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var consumeResult = _consumer.Consume(cancellationToken);
                if (consumeResult is null)
                {
                    return;
                }

                var message = JsonSerializer.Deserialize<RewardGrantedMessage>(consumeResult.Message.Value);
                Console.WriteLine($"Received message RewardGranted: " + message);

                var command = new RewardPlayerCommand(message.PlayerId.ToString(), message.Credits, message.Experience, message.Difficulty);
                await _mediator.Send(command);
            }
        }, cancellationToken);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _consumer.Unsubscribe();
        return Task.CompletedTask;
    }
}