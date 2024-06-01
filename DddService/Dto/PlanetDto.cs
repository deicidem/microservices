using DddService.Aggregates.MissionNamespace;
using Microsoft.OpenApi.Extensions;

namespace DddService.Dto;

public record PlanetDto(string Id, string Name, string Status, int Progress)
{

    public static PlanetDto From(Planet planet)
    {

        return new PlanetDto(
            planet.Id.ToString(),
            planet.Name,
            planet.Status.GetDisplayName(),
            planet.Progress
        );
    }
}
