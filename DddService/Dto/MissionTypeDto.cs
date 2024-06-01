using DddService.Aggregates.MissionNamespace;

namespace DddService.Dto;

public record MissionTypeDto(string Id, string Name, string Description, IEnumerable<Goal> Goals)
{
    public static MissionTypeDto From(MissionType missionType)
    {
        return new MissionTypeDto(missionType.Id.ToString(), missionType.Name.Value, missionType.Description.Value, missionType.Goals.GetGoals());
    }
}
