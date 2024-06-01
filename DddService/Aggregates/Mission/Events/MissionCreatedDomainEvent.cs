using DddService.Aggregates.PlayerNamespace;
using DddService.Common;

namespace DddService.Aggregates.MissionNamespace;

public record MissionCreatedDomainEvent
    (Guid Id, Guid PlayerId, string TypeName, string PlanetName, Difficulty Difficulty) : IDomainEvent;
