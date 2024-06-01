using System.Text.Json;
using DddService.Aggregates.MissionNamespace;
using DddService.EventBus;
using MediatR;
using Messages;

public class MissionCreatedDomainEventHandler : INotificationHandler<MissionCreatedDomainEvent>
{
    private readonly KafkaProducerService _kafkaProducerService;

    public MissionCreatedDomainEventHandler(KafkaProducerService kafkaProducerService)
    {
        _kafkaProducerService = kafkaProducerService;
    }

    public async Task Handle(MissionCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        var missionCreatedMessage = new MissionCreatedMessage(notification.Id, notification.PlayerId, notification.TypeName, notification.PlanetName, notification.Difficulty, new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString());
        Console.WriteLine($"Mission has been created at {DateTime.Now}: " + JsonSerializer.Serialize(missionCreatedMessage));
        await _kafkaProducerService.ProduceAsync("NewMissionCreated", JsonSerializer.Serialize(missionCreatedMessage));
    }
}