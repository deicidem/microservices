using System.Collections;
using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using DddService.Aggregates.PlayerNamespace;

namespace DddService.Aggregates.CommandCenterNamespace;

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
public record PlanetName
{
    public string Value { get; }

    private PlanetName(string value)
    {
        Value = value;
    }

    public static PlanetName Of(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new Exception();
        }

        return new PlanetName(value);
    }

    public static implicit operator string(PlanetName name)
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
public record DifficultyName
{
    public string Value { get; }

    private DifficultyName(string value)
    {
        Value = value;
    }

    public static DifficultyName Of(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new Exception();
        }

        return new DifficultyName(value);
    }

    public static implicit operator string(DifficultyName name)
    {
        return name.Value;
    }
}
public record DifficultyLevel
{
    public int Value { get; }
    private static readonly int MaxLevel = 9;

    private DifficultyLevel(int value)
    {
        Value = value;
    }

    public static DifficultyLevel Of(int value)
    {
        if (value < 1 || value > MaxLevel)
        {
            throw new Exception();
        }

        return new DifficultyLevel(value);
    }

    public static implicit operator int(DifficultyLevel lvl)
    {
        return lvl.Value;
    }
}

public class MissionId
{
    public Guid Value { get; }

    private MissionId(Guid value)
    {
        Value = value;
    }

    public static MissionId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new Exception();
        }

        return new MissionId(value);
    }

    public static implicit operator Guid(MissionId id)
    {
        return id.Value;
    }
}

public record Reinforcements
{
    public int Value { get; }

    private Reinforcements(int value)
    {
        Value = value;
    }
    public static Reinforcements Default()
    {
        return new Reinforcements(20);
    }
    public static Reinforcements Of(int value)
    {
        if (value < 0 || value > 30)
        {
            throw new Exception();
        }

        return new Reinforcements(value);
    }

    public static implicit operator int(Reinforcements reinforcements)
    {
        return reinforcements.Value;
    }
}

public class MissionTypeId
{
    public Guid Value { get; }

    private MissionTypeId(Guid value)
    {
        Value = value;
    }

    public static MissionTypeId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new Exception();
        }

        return new MissionTypeId(value);
    }

    public static implicit operator Guid(MissionTypeId id)
    {
        return id.Value;
    }
}
public class ObjectiveId
{
    public Guid Value { get; }

    private ObjectiveId(Guid value)
    {
        Value = value;
    }

    public static ObjectiveId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new Exception();
        }

        return new ObjectiveId(value);
    }

    public static implicit operator Guid(ObjectiveId id)
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
public record Description
{
    public string Value { get; }

    private Description(string value)
    {
        Value = value;
    }

    public static Description Of(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new Exception();
        }

        return new Description(value);
    }

    public static implicit operator string(Description description)
    {
        return description.Value;
    }
}

public record Goal
{
    public string Value { get; }

    private Goal(string value)
    {
        Value = value;
    }

    public static Goal Of(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new Exception();
        }

        return new Goal(value);
    }


}
public class IsCompleted
{
    public bool Value { get; }

    private IsCompleted(bool value)
    {
        Value = value;
    }

    public static IsCompleted Of(bool value)
    {
        return new IsCompleted(value);
    }

    public static implicit operator bool(IsCompleted isCompleted)
    {
        return isCompleted.Value;
    }
}
public record Goals
{
    public string Value { get; }

    private Goals(string value)
    {
        Value = value;
    }

    public static Goals Of(List<Goal> value)
    {
        if (value is null || value.Count == 0)
        {
            throw new Exception("List of goals cannot be null or empty");
        }

        string jsonValue = JsonSerializer.Serialize(value);
        return new Goals(jsonValue);
    }

    public static Goals Of(string json)
    {
        if (string.IsNullOrEmpty(json))
        {
            throw new Exception("JSON value cannot be empty");
        }

        return new Goals(json);
    }

    public static implicit operator string(Goals goals)
    {
        return goals.Value;
    }

    public List<Goal> GetGoals()
    {
        var goals = JsonSerializer.Deserialize<List<Goal>>(Value);
        if (goals is null || goals.Count == 0)
        {
            throw new Exception("List of goals cannot be null or empty");
        }
        return goals;
    }
}

public record Time
{
    public int Value { get; }

    private Time(int value)
    {
        Value = value;
    }

    public static Time Of(int value)
    {
        if (value < 0 || value > 60)
        {
            throw new Exception();
        }

        return new Time(value);
    }

    public static implicit operator int(Time time)
    {
        return time.Value;
    }
}

public record Squad
{
    public string Value { get; }

    private Squad(string value)
    {
        Value = value;
    }

    public static Squad Of(List<PlayerId> value)
    {
        if (value is null || value.Count == 0 || value.Count > 4)
        {
            throw new Exception();
        }

        string jsonValue = JsonSerializer.Serialize(value);
        return new Squad(jsonValue);
    }

    public static Squad Of(string json)
    {
        if (string.IsNullOrEmpty(json))
        {
            throw new Exception("JSON value cannot be empty");
        }

        return new Squad(json);
    }
    public static implicit operator string(Squad squad)
    {
        return squad.Value;
    }

    public List<PlayerId> GetPlayers()
    {
        var res = JsonSerializer.Deserialize<List<PlayerId>>(Value);
        if (res is null || res.Count == 0 || res.Count > 4)
        {
            throw new Exception();
        }
        return res;
    }
}

public record StartedAt
{
    public DateTime Value { get; }

    private StartedAt(DateTime value)
    {
        Value = value;
    }

    public static StartedAt Now()
    {
        return new StartedAt(DateTime.Now);
    }

    public static implicit operator DateTime(StartedAt time)
    {
        return time.Value;
    }
}

public record FinishedAt
{
    public DateTime Value { get; }

    private FinishedAt(DateTime value)
    {
        Value = value;
    }

    public static FinishedAt Now()
    {
        return new FinishedAt(DateTime.Now);
    }

    public static implicit operator DateTime(FinishedAt time)
    {
        return time.Value;
    }
}

public enum MissionStatus
{
    Initiated = 1,
    InProgress = 2,
    Abandoned = 3,
    Completed = 4,
    Failed = 5,
}


public class CommandCenterId
{
    public Guid Value { get; }

    private CommandCenterId(Guid value)
    {
        Value = value;
    }

    public static CommandCenterId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new Exception();
        }

        return new CommandCenterId(value);
    }

    public static implicit operator Guid(CommandCenterId id)
    {
        return id.Value;
    }
}
