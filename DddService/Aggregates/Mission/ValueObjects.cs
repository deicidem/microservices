using System.Collections;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace DddService.Aggregates.MissionNamespace;

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
    public bool IsCompleted { get; private set; }

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

public record Goals
{
    public List<Goal> Value { get; }

    private Goals(List<Goal> value)
    {
        Value = value;
    }

    public static Goals Of(List<Goal> value)
    {
        if (value is null || value.Count == 0)
        {
            throw new Exception();
        }

        return new Goals(value);
    }

    public static implicit operator List<Goal>(Goals objectives)
    {
        return objectives.Value;
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

public class InitiatorId
{
    public Guid Value { get; }

    private InitiatorId(Guid value)
    {
        Value = value;
    }

    public static InitiatorId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new Exception();
        }

        return new InitiatorId(value);
    }

    public static implicit operator Guid(InitiatorId id)
    {
        return id.Value;
    }
}

public class Squad
{
    public List<Guid> Value { get; }

    private Squad(List<Guid> value)
    {
        Value = value;
    }

    public static Squad Of(List<Guid> value)
    {
        if (value is null || value.Count == 0)
        {
            throw new Exception();
        }

        return new Squad(value);
    }

    public static implicit operator List<Guid>(Squad squad)
    {
        return squad.Value;
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
