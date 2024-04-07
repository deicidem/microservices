using System.Collections;
using DddService.Aggregates.PlayerNamespace;
using DddService.Common;

namespace DddService.Aggregates.CommandCenterNamespace;

public class Planet : Entity<PlanetId>
{
    public PlanetName Name { get; set; } = default!;
    public LiberationProgress Progress { get; set; } = default!;
    public LiberationStatus Status { get; set; } = default!;

    public static Planet Create(PlanetId id, PlanetName name, LiberationStatus status)
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

public class Difficulty : Entity<DifficultyId>
{
    public DifficultyName Name { get; set; } = default!;
    public DifficultyLevel Level { get; set; } = default!;

    public static Difficulty Create(DifficultyId id, DifficultyName name, DifficultyLevel level)
    {
        return new Difficulty
        {
            Id = id,
            Name = name,
            Level = level
        };
    }
}

public class Objective : Entity<ObjectiveId>
{
    public Goal Goal { get; private set; } = default!;
    public IsCompleted IsCompleted { get; private set; } = default!;
    public Mission Mission { get; private set; } = default!;
    public MissionId MissionId { get; private set; } = default!;
    public static Objective Create(ObjectiveId id, Goal goal, Mission mission)
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
public class MissionType : Entity<MissionTypeId>
{
    public Name Name { get; private set; } = default!;
    public Description Description { get; private set; } = default!;
    public Goals Goals { get; private set; } = default!;

    public static MissionType Create(MissionTypeId id, Name name, Description description, Goals goals)
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

public class Mission : Entity<MissionId>
{
    public MissionType Type { get; set; } = default!;
    public MissionTypeId TypeId { get; set; } = default!;
    public MissionStatus Status { get; set; } = default!;
    public ICollection<Objective> Objectives { get; set; } = default!;
    public Player Initiator { get; set; } = default!;
    public PlayerId InitiatorId { get; set; } = default!;

    public Difficulty Difficulty { get; set; } = default!;
    public DifficultyId DifficultyId { get; set; } = default!;
    public Planet Planet { get; set; } = default!;
    public PlanetId PlanetId { get; set; } = default!;
    public Squad Squad { get; set; } = default!;
    public Reinforcements Reinforcements { get; set; } = default!;

    public static Mission Create(MissionId id, MissionType type, Player initiator, Difficulty difficulty, Planet planet)
    {
        var mission = new Mission
        {
            Id = id,
            Type = type,
            Planet = planet,
            Difficulty = difficulty,
            Initiator = initiator,
            Status = MissionStatus.Initiated,
            Squad = Squad.Of([initiator.Id])
        };
        var objectives = type.Goals.GetGoals().Select(g => Objective.Create(
            ObjectiveId.Of(Guid.NewGuid()),
            g,
            mission
        )).ToList();

        return mission;
    }

    public void AddToSquad(Player player)
    {
        Squad = Squad.Of([.. Squad.GetPlayers(), player.Id]);
    }

    public void RemoveFromSquad(Player player)
    {
        if (Squad.GetPlayers().Count == 1)
        {
            UpdateStatus(MissionStatus.Abandoned);
        }
        else
        {
            Squad = Squad.Of([.. Squad.GetPlayers().Where(x => x != player.Id)]);
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
    }

    private void UpdateStatus(MissionStatus status)
    {
        Status = status;
    }
}

public class CommandCenter : Aggregate<CommandCenterId>
{
    public Player Player { get; set; } = default!;
    public PlayerId PlayerId { get; set; } = default!;
    public Planet Planet { get; set; } = default!;
    public PlanetId PlanetId { get; set; } = default!;
    public Mission Mission { get; set; } = default!;
    public MissionId MissionId { get; set; } = default!;
    public Difficulty Difficulty { get; set; } = default!;
    public DifficultyId DifficultyId { get; set; } = default!;
    public Difficulty HighestDifficultyAvailable { get; set; } = default!;
    public DifficultyId HighestDifficultyAvailableId { get; set; } = default!;
    public static CommandCenter Create(CommandCenterId id, Player player, Difficulty startDifficulty)
    {
        return new CommandCenter
        {
            Id = id,
            Player = player,
            HighestDifficultyAvailable = startDifficulty,
        };
    }
    public ICollection<Planet> OpenMap()
    {
        return new List<Planet> { Planet };
    }
    public void SelectDifficulty(Difficulty difficulty)
    {
        Difficulty = difficulty;
    }
    public void UpdateHighestDifficultyAvailable(Difficulty difficulty)
    {
        HighestDifficultyAvailable = difficulty;
    }
    public void SelectPlanet(Planet planet)
    {
        Planet = planet;
    }
}
