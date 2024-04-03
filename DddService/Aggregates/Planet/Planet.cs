using System.Collections;
using DddService.Common;

namespace DddService.Aggregates.PlanetNamespace;
public class Planet : Entity<PlanetId>
{
    public Name Name { get; private set; } = default!;
    public LiberationProgress Progress { get; private set; } = default!;
    public LiberationStatus Status { get; private set; } = default!;
    public CurrentPlayers CurrentPlayers { get; private set; } = default!;

    public static Planet Create(Name name, LiberationProgress progress)
    {
        return new Planet
        {
            Name = name,
            Progress = progress,
            Status = progress == 0 ? LiberationStatus.Occupied : progress == 100 ? LiberationStatus.Liberated : LiberationStatus.InProgress,
            CurrentPlayers = CurrentPlayers.Of(0)
        };
    }
}
