using DddService.Aggregates.MissionNamespace;

namespace Messages;

public record MissionStartedMessage(
    Guid Id,
    string Difficulty,
    string Status,
    int Reinforcements,
    string Timestamp
);
