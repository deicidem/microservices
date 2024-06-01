using System.Text.Json;
using DddService.Aggregates.MissionNamespace;
using DddService.EventBus;
using MediatR;
using Messages;
using Microsoft.OpenApi.Extensions;

public class MissionCompleteedDomainEventHandler : INotificationHandler<MissionCompletedDomainEvent>
{
    private readonly KafkaProducerService _kafkaProducerService;

    public MissionCompleteedDomainEventHandler(KafkaProducerService kafkaProducerService)
    {
        _kafkaProducerService = kafkaProducerService;
    }

    public async Task Handle(MissionCompletedDomainEvent notification, CancellationToken cancellationToken)
    {
        var missionCompleteedMessage = new MissionCompletedMessage(notification.Id, notification.Difficulty.GetDisplayName(), notification.Status.GetDisplayName(), notification.Reinforcements, notification.Squad, new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString());

        Console.WriteLine($"Mission has been Completed at {DateTime.Now}: " + JsonSerializer.Serialize(missionCompleteedMessage));

        await _kafkaProducerService.ProduceAsync("MissionCompleted", JsonSerializer.Serialize(missionCompleteedMessage));
    }
}