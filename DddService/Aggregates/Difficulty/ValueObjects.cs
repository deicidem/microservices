using System.Collections;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace DddService.Aggregates.DifficultyNamespace;

public class DifficultyId
{
    public Guid Value { get; }

    private DifficultyId(Guid value)
    {
        Value = value;
    }

    public static DifficultyId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new Exception();
        }

        return new DifficultyId(value);
    }

    public static implicit operator Guid(DifficultyId id)
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

public record Level
{
    public int Value { get; }
    private static readonly int MaxLevel = 9;

    private Level(int value)
    {
        Value = value;
    }

    public static Level Of(int value)
    {
        if (value < 1 || value > MaxLevel)
        {
            throw new Exception();
        }

        return new Level(value);
    }

    public static implicit operator int(Level lvl)
    {
        return lvl.Value;
    }
}
