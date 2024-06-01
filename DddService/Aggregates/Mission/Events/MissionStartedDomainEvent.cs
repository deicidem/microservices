using DddService.Aggregates.PlayerNamespace;
using DddService.Common;

namespace DddService.Aggregates.MissionNamespace;

public record MissionStartedDomainEvent(
    Guid Id,
    Difficulty Difficulty,
    MissionStatus Status,
    int Reinforcements
) : IDomainEvent;
