using DddService.Aggregates.MissionNamespace;

namespace DddService.Dto;

public record MissionTypeDto(string Id, string Name, string Description, IEnumerable<Goal> Goals);
