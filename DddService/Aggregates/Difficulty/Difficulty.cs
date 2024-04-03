using System.Collections;
using DddService.Common;

namespace DddService.Aggregates.DifficultyNamespace;
public class Difficulty : Entity<DifficultyId>
{
    public Name Name { get; private set; } = default!;
    public Level Level { get; private set; } = default!;
    public static Difficulty Create(Name name, Level level)
    {
        return new Difficulty
        {
            Name = name,
            Level = level
        };
    }
}
