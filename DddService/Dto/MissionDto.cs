using DddService.Aggregates.MissionNamespace;

namespace DddService.Dto;

public record MissionDto(string Id, string Name, int Level);
public record MissionInitiateModel(string MissionTypeId, string PlanetId, Difficulty Difficulty);
