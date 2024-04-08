namespace DddService.Dto;

public record CommandCenterDto(string Id, string PlayerId, string? MissionId, string? PlanetId, string? DifficultyId, string HighestDifficultyAvailableId);
