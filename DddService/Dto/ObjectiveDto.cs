using DddService.Aggregates.MissionNamespace;
using Microsoft.OpenApi.Extensions;

namespace DddService.Dto;

public record ObjectiveDto(string Id, string Goal, bool IsCompleted, string MissionId)
{

    public static ObjectiveDto From(Objective objective)
    {
        return new ObjectiveDto(objective.Id.ToString(), objective.Goal.Value, objective.IsCompleted.Value, objective.Mission.Id.ToString());
    }
}
