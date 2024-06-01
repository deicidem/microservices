using System.Text.Json;
using DddService.Aggregates.MissionNamespace;
using DddService.EventBus;
using MediatR;
using Messages;
using Microsoft.OpenApi.Extensions;

public class MissionStartedDomainEventHandler : INotificationHandler<MissionStartedDomainEvent>
{
    private readonly KafkaProducerService _kafkaProducerService;

    public MissionStartedDomainEventHandler(KafkaProducerService kafkaProducerService)
    {
        _kafkaProducerService = kafkaProducerService;
    }

    public async Task Handle(MissionStartedDomainEvent notification, CancellationToken cancellationToken)
    {
        var missionStartedMessage = new MissionStartedMessage(notification.Id, notification.Difficulty.GetDisplayName(), notification.Status.GetDisplayName(), notification.Reinforcements, new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString());
        Console.WriteLine($"Mission has been Started at {DateTime.Now}: " + JsonSerializer.Serialize(missionStartedMessage));
        await _kafkaProducerService.ProduceAsync("NewMissionStarted", JsonSerializer.Serialize(missionStartedMessage));
    }
}