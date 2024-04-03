using System.Collections;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace DddService.Aggregates.PlanetNamespace;

public class PlanetId
{
    public Guid Value { get; }

    private PlanetId(Guid value)
    {
        Value = value;
    }

    public static PlanetId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new Exception();
        }

        return new PlanetId(value);
    }

    public static implicit operator Guid(PlanetId id)
    {
        return id.Value;
    }
}

public record Name
{
    public string Value { get; }

    private Name(string value)
    {
        Value = value;
    }

    public static Name Of(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new Exception();
        }

        return new Name(value);
    }

    public static implicit operator string(Name name)
    {
        return name.Value;
    }
}

public record LiberationProgress
{
    public int Value { get; }

    private LiberationProgress(int value)
    {
        Value = value;
    }

    public static LiberationProgress Of(int value)
    {
        if (value < 0 || value > 100)
        {
            throw new Exception();
        }

        return new LiberationProgress(value);
    }

    public static implicit operator int(LiberationProgress progress)
    {
        return progress.Value;
    }
}

public enum LiberationStatus
{
    Occupied = 1,
    InProgress = 2,
    Liberated = 3,
}

public record CurrentPlayers
{
    public int Value { get; }

    private CurrentPlayers(int value)
    {
        Value = value;
    }

    public static CurrentPlayers Of(int value)
    {
        if (value < 0)
        {
            throw new Exception();
        }

        return new CurrentPlayers(value);
    }

    public static implicit operator int(CurrentPlayers progress)
    {
        return progress.Value;
    }
}
