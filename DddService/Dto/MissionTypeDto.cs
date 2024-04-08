namespace DddService.Dto;

public record MissionTypeDto(string Id, string Name, string Description, IEnumerable<string> Goals);
