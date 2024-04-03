using System.Collections;
using DddService.Common;

namespace DddService.Aggregates.PlayerNamespace;
public class Player : Entity<PlayerId>
{
    public Name Name { get; private set; } = default!;
    public Rank Rank { get; private set; } = default!;
    public Credits Credits { get; private set; } = default!;
    public Experience Experience { get; private set; } = default!;
    public static Player Create(Name name)
    {
        return new Player
        {
            Name = name,
            Credits = Credits.Of(0),
            Experience = Experience.Of(0),
            Rank = Rank.Of(0)
        };
    }

    public void GainCredits(Credits credits)
    {
        Credits = Credits.Of(Credits.Value + credits.Value);
    }
    public void GainExperience(Experience experience)
    {
        Experience = Experience.Of(Experience.Value + experience.Value);
    }
}
