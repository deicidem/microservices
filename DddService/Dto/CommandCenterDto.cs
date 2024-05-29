namespace DddService.Dto;

public record CommandCenterDto(string Id, string PlayerId, string? MissionId, string HighestDifficultyAvailableId);
