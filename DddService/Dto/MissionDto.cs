using DddService.Aggregates.MissionNamespace;
using Microsoft.OpenApi.Extensions;

namespace DddService.Dto;

public record MissionDto(
    string Id,
    string Status,
    string Difficulty,
    int Reinforcements,
    string InitiatorId,
    MissionTypeDto MissionType,
    PlanetDto Planet,
    List<string> Squad,
    List<ObjectiveDto> Objectives
)
{
    public static MissionDto From(Mission mission)
    {
        return new MissionDto(
            mission.Id.ToString(),
            mission.Status.GetDisplayName(),
            mission.Difficulty.GetDisplayName(),
            mission.Reinforcements,
            mission.InitiatorId.ToString(),
            MissionTypeDto.From(mission.Type),
            PlanetDto.From(mission.Planet),
            mission.Squad.GetPlayers().Select(p => p.ToString()).ToList(),
            mission.Objectives.Select(o => ObjectiveDto.From(o)).ToList()
        );
    }
};
public record MissionInitiateModel(string MissionTypeId, string PlanetId, Difficulty Difficulty);
