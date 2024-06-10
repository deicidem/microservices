using DddService.Aggregates.PlayerNamespace;
using DddService.Common;

namespace DddService.Aggregates.MissionNamespace;

public class Planet : Entity
{
    public PlanetName Name { get; set; } = default!;
    public LiberationProgress Progress { get; set; } = default!;
    public LiberationStatus Status { get; set; } = default!;

    public static Planet Create(Guid id, PlanetName name, LiberationStatus status)
    {
        if (status == LiberationStatus.InProgress)
        {
            throw new Exception();
        }
        var progress = LiberationProgress.Of(0);

        if (status == LiberationStatus.Liberated)
        {
            progress = LiberationProgress.Of(100);
        }

        if (status == LiberationStatus.Occupied)
        {
            progress = LiberationProgress.Of(0);
        }

        return new Planet
        {
            Id = id,
            Name = name,
            Status = status,
            Progress = progress,
        };
    }

    public void UpdateProgress(LiberationProgress progress)
    {
        if (progress == 100)
        {
            Status = LiberationStatus.Liberated;
        }
        else if (progress == 0)
        {
            Status = LiberationStatus.Occupied;
        }

        Progress = progress;
    }
}


public class Objective : Entity
{
    public Goal Goal { get; private set; } = default!;
    public IsCompleted IsCompleted { get; private set; } = default!;
    public Guid MissionId { get; private set; } = default!;
    public Mission Mission { get; private set; } = default!;
    public static Objective Create(Guid id, Goal goal, Mission mission)
    {
        return new Objective
        {
            Id = id,
            Goal = goal,
            Mission = mission,
            IsCompleted = IsCompleted.Of(false)
        };
    }
    public void Complete()
    {
        IsCompleted = IsCompleted.Of(true);
    }
}
public class MissionType : Entity
{
    public Name Name { get; private set; } = default!;
    public Description Description { get; private set; } = default!;
    public Goals Goals { get; private set; } = default!;

    public static MissionType Create(Guid id, Name name, Description description, Goals goals)
    {
        return new MissionType
        {
            Id = id,
            Name = name,
            Description = description,
            Goals = goals
        };
    }
}

public class Mission : Aggregate
{
    public MissionType Type { get; set; } = default!;
    public Guid TypeId { get; set; } = default!;
    public MissionStatus Status { get; set; } = default!;
    public ICollection<Objective> Objectives { get; set; } = default!;
    public Player Initiator { get; set; } = default!;
    public Guid InitiatorId { get; set; } = default!;

    public Difficulty Difficulty { get; set; } = default!;
    public Planet Planet { get; set; } = default!;
    public Guid PlanetId { get; set; } = default!;
    public Squad Squad { get; set; } = default!;
    public Reinforcements Reinforcements { get; set; } = default!;

    public static Mission Create(Guid id, MissionType type, Player initiator, Difficulty difficulty, Planet planet)
    {
        var mission = new Mission
        {
            Id = id,
            Type = type,
            Planet = planet,
            Difficulty = difficulty,
            Initiator = initiator,
            Status = MissionStatus.Initiated,
            Squad = Squad.Of([initiator.Id]),
            Reinforcements = Reinforcements.Default(),
        };

        mission.Objectives = type.Goals.GetGoals().Select(g => Objective.Create(
            Guid.NewGuid(),
            g,
            mission
        )).ToList();

        var @event = new MissionCreatedDomainEvent(
            mission.Id,
            initiator.Id,
            type.Name,
            planet.Name,
            difficulty
        );

        mission.AddDomainEvent(@event);

        return mission;
    }

    public void AddToSquad(Player player)
    {
        if (Squad.GetPlayers().Count >= 4)
        {
            throw new Exception();
        }
        Squad = Squad.Of([.. Squad.GetPlayers(), player.Id]);
    }

    public void RemoveFromSquad(Player player)
    {
        if (Squad.GetPlayers().Count == 1)
        {
            Finish(MissionStatus.Abandoned);
        }
        else
        {
            Squad = Squad.Of([.. Squad.GetPlayers().Where(x => x != player.Id)]);
            if (InitiatorId.Equals(player.Id))
            {
                InitiatorId = Squad.GetPlayers().First();
            }
        }
    }

    public void Start()
    {
        if (Status != MissionStatus.Initiated)
        {
            throw new Exception();
        }

        if (Squad.GetPlayers().Count == 0)
        {
            throw new Exception();
        }

        UpdateStatus(MissionStatus.InProgress);

        var @event = new MissionStartedDomainEvent(
            Id,
            Difficulty,
            Status,
            Reinforcements,
            Squad.GetPlayers()
        );

        AddDomainEvent(@event);
    }

    public void Finish(MissionStatus status)
    {
        if (Status != MissionStatus.InProgress)
        {
            throw new Exception();
        }

        if (status == MissionStatus.Initiated || status == MissionStatus.InProgress)
        {
            throw new Exception();
        }

        UpdateStatus(status);

        var @event = new MissionCompletedDomainEvent(
            Id,
            Difficulty,
            Status,
            Reinforcements,
            Squad.GetPlayers(),
            Objectives.Where(o => o.IsCompleted == IsCompleted.Of(true).Value).ToList().Count
        );

        AddDomainEvent(@event);
    }

    public void CompleteObjective()
    {
        if (Objectives.Count == 0)
        {
            throw new Exception();
        }

        var objective = Objectives.First(o => o.IsCompleted == IsCompleted.Of(false).Value);
        objective.Complete();

        if (Objectives.All(o => o.IsCompleted == IsCompleted.Of(true).Value))
        {
            Finish(MissionStatus.Completed);
        }
    }

    private void UpdateStatus(MissionStatus status)
    {
        Status = status;
    }
}