using DddService.Aggregates.CommandCenterNamespace;

namespace DddService.Dto;

public record CommandCenterDto(string Id, string PlayerId, string? MissionId, string HighestDifficultyAvailableId)
{
    public static CommandCenterDto From(CommandCenter commandCenter)
    {
        return new CommandCenterDto(commandCenter.Id.ToString(), commandCenter.PlayerId.ToString(), commandCenter.MissionId?.ToString(), commandCenter.HighestDifficultyAvailable.ToString());
    }
}
