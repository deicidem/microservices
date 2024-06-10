using DddService.Aggregates.MissionNamespace;

namespace Messages;

public record MissionStartedMessage(
    Guid Id,
    Difficulty Difficulty,
    string Status,
    int Reinforcements,
    List<Guid> Squad,
    string Timestamp
);
