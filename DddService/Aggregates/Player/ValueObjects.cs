using System.Collections;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
namespace DddService.Aggregates.PlayerNamespace;

public class PlayerId
{
    public Guid Value { get; }

    private PlayerId(Guid value)
    {
        Value = value;
    }

    public static PlayerId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new Exception();
        }

        return new PlayerId(value);
    }

    public static implicit operator Guid(PlayerId id)
    {
        return id.Value;
    }
}

public class Experience
{
    public int Value { get; }

    private Experience(int value)
    {
        Value = value;
    }

    public static Experience Of(int value)
    {
        if (value < 0)
        {
            throw new Exception();
        }

        return new Experience(value);
    }

    public static implicit operator int(Experience xp)
    {
        return xp.Value;
    }
}

public class Credits
{
    public int Value { get; }

    private Credits(int value)
    {
        Value = value;
    }

    public static Credits Of(int value)
    {
        if (value < 0)
        {
            throw new Exception();
        }

        return new Credits(value);
    }

    public static implicit operator int(Credits xp)
    {
        return xp.Value;
    }
}

public record Nickname
{
    public string Value { get; }

    private Nickname(string value)
    {
        Value = value;
    }

    public static Nickname Of(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new Exception();
        }

        return new Nickname(value);
    }

    public static implicit operator string(Nickname name)
    {
        return name.Value;
    }
}

public record Rank
{
    public string Value { get; }
    private static readonly Dictionary<int, string> RanksMap = new() {
        { 0, "Cadet" },
        { 1000, "Solider" },
        { 5000, "Captain" },
        { 10000, "Commander" },
    };

    private Rank(string value)
    {
        Value = value;
    }

    public static Rank Of(int value)
    {
        if (value < 0)
        {
            throw new Exception();
        }
        string finalValue = string.Empty;

        RanksMap.Keys
            .OrderByDescending(x => x)
            .ToList()
            .ForEach(x =>
            {
                if (value >= x && finalValue == string.Empty)
                {
                    finalValue = RanksMap[x];
                }
            });

        if (finalValue == string.Empty)
        {
            throw new Exception();
        }

        return new Rank(finalValue);
    }

    public static implicit operator string(Rank name)
    {
        return name.Value;
    }
}
