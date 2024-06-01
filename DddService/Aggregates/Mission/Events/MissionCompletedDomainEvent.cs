using DddService.Aggregates.PlayerNamespace;
using DddService.Common;

namespace DddService.Aggregates.MissionNamespace;

public record MissionCompletedDomainEvent(
    Guid Id,
    Difficulty Difficulty,
    MissionStatus Status,
    int Reinforcements,
    List<Guid> Squad
) : IDomainEvent;
