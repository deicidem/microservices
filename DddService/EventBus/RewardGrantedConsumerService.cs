using System.Text.Json;
using Confluent.Kafka;
using DddService.Features.CommandCenterFeature;
using MediatR;
using Messages;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DddService.EventBus;

public class RewardGrantedConsumerService : IHostedService
{
    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _serviceProvider;
    private readonly IConsumer<string, string> _consumer;
    private readonly ILogger<RewardGrantedConsumerService> _logger;
    private Task _executingTask;
    private readonly CancellationTokenSource _cts = new CancellationTokenSource();

    public RewardGrantedConsumerService(IConfiguration configuration, IServiceProvider serviceProvider, ILogger<RewardGrantedConsumerService> logger)
    {
        _configuration = configuration;
        _serviceProvider = serviceProvider;
        _logger = logger;

        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = _configuration["Kafka:BootstrapServers"],
            GroupId = "consumer-group-1",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        _consumer = new ConsumerBuilder<string, string>(consumerConfig).Build();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("RewardGrantedConsumerService is starting.");

        _consumer.Subscribe("RewardGranted");

        _executingTask = Task.Run(() => ExecuteAsync(_cts.Token), cancellationToken);

        return Task.CompletedTask;
    }

    private async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = _consumer.Consume(cancellationToken);
                    if (consumeResult != null)
                    {
                        var message = JsonSerializer.Deserialize<RewardGrantedMessage>(consumeResult.Message.Value);
                        _logger.LogInformation($"Received message RewardGranted: {message}");

                        using (var scope = _serviceProvider.CreateScope())
                        {
                            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                            var command = new RewardPlayerCommand(message.PlayerId.ToString(), message.Experience, message.Credits, message.Difficulty);
                            await mediator.Send(command, cancellationToken);
                        }
                    }
                }
                catch (ConsumeException ex)
                {
                    _logger.LogError($"Consume error: {ex.Error.Reason}");
                }
                catch (JsonException ex)
                {
                    _logger.LogError($"JSON deserialize error: {ex.Message}");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Unexpected error: {ex.Message}");
                }
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Cancellation requested.");
        }
        finally
        {
            _consumer.Close();
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("RewardGrantedConsumerService is stopping.");

        _cts.Cancel();

        return Task.WhenAny(_executingTask, Task.Delay(Timeout.Infinite, cancellationToken));
    }
}