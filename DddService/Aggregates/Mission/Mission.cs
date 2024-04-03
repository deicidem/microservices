using System.Collections;
using DddService.Aggregates.DifficultyNamespace;
using DddService.Aggregates.PlanetNamespace;
using DddService.Aggregates.PlayerNamespace;
using DddService.Common;

namespace DddService.Aggregates.MissionNamespace;
public class Objective : Entity<ObjectiveId>
{
    public string Goal { get; private set; } = default!;
    public bool IsCompleted { get; private set; }
    public static Objective Create(Goal goal)
    {
        return new Objective
        {
            Goal = goal.Value,
            IsCompleted = false
        };
    }
    public void Complete()
    {
        IsCompleted = true;
    }
}
public class MissionType : Entity<MissionTypeId>
{
    public Name Name { get; private set; } = default!;
    public Description Description { get; private set; } = default!;
    public Goals Goals { get; private set; } = default!;

    public static MissionType Create(Name name, Description description, Goals goals)
    {
        return new MissionType
        {
            Name = name,
            Description = description,
            Goals = goals
        };
    }
}

public class Mission : Aggregate<MissionId>
{
    public MissionType Type { get; private set; } = default!;
    public Time Time { get; private set; } = default!;
    public PlanetId PlanetId { get; private set; } = default!;
    public DifficultyId DifficultyId { get; private set; } = default!;
    public PlayerId InitiatorId { get; private set; } = default!;
    public StartedAt StartedAt { get; private set; } = default!;
    public MissionStatus Status { get; private set; } = default!;
    public FinishedAt FinishedAt { get; private set; } = default!;
    public ICollection<PlayerId> Squad { get; private set; } = default!;
    public ICollection<Objective> Objectives { get; private set; } = default!;

    public static Mission Initiate(PlanetId planetId, DifficultyId difficultyId, PlayerId initiatorId)
    {
        // Будет браться случайный тип миссии
        var destroyBaseMission = MissionType.Create(
            Name.Of("Destroy base"),
            Description.Of("Destroy enemy bases on north and south"),
            Goals.Of([Goal.Of("Destroy enemy bases on north"), Goal.Of("Destroy enemy bases on south")])
        );

        var objectives = destroyBaseMission.Goals.Value.Select(Objective.Create).ToList();

        // Проверить доступна ли сложность игроку

        return new Mission
        {
            Type = destroyBaseMission,
            Objectives = objectives,
            PlanetId = planetId,
            DifficultyId = difficultyId,
            InitiatorId = initiatorId,
            Squad = [initiatorId]
        };
    }
    public void Start(PlayerId playerId)
    {
        if (playerId != InitiatorId)
        {
            throw new Exception();
        }
        StartedAt = StartedAt.Now();
    }

    public void Join(PlayerId playerId)
    {
        if (Squad.Count >= 4)
        {
            throw new Exception();
        }
        if (Squad.Contains(playerId))
        {
            throw new Exception();
        }
        Squad.Add(playerId);
    }

    public void Abandon(PlayerId playerId)
    {
        if (!Squad.Contains(playerId))
        {
            throw new Exception();
        }

        Squad.Remove(playerId);

        if (playerId == InitiatorId)
        {
            Squad.ToList().ForEach(Abandon);
        }

        if (Squad.Count == 0)
        {
            Fail();
        }

    }

    public void Fail()
    {
        if (Status != MissionStatus.InProgress)
        {
            throw new Exception();
        }
        Status = MissionStatus.Failed;
        FinishedAt = FinishedAt.Now();
        RewardSquad();
    }

    public void CompleteObjective(ObjectiveId objectiveId)
    {
        if (!Objectives.Any(x => x.Id == objectiveId))
        {
            throw new Exception();
        }

        Objectives.First(x => x.Id == objectiveId).Complete();
    }

    public void Complete()
    {
        if (Status != MissionStatus.InProgress)
        {
            throw new Exception();
        }
        if (!Objectives.All(o => o.IsCompleted))
        {
            throw new Exception();
        }

        Status = MissionStatus.Completed;
        FinishedAt = FinishedAt.Now();
        RewardSquad();
        // Изменить статус осовобождение планеты (событие)
    }

    private void RewardSquad()
    {
        Credits credits;
        Experience experience;
        var completedObjectives = Objectives.ToList().Count(o => o.IsCompleted);
        // GetDifficultyById
        var difficulty = Difficulty.Create(DifficultyNamespace.Name.Of("Easy"), Level.Of(1));

        credits = Credits.Of(completedObjectives * 100 * difficulty.Level);
        experience = Experience.Of(completedObjectives * 200 * difficulty.Level);

        // GetPlayerById
        Squad.ToList().ForEach(x => RewardPlayer(Player.Create(PlayerNamespace.Name.Of("Player")), credits, experience));
    }

    private static void RewardPlayer(Player player, Credits credits, Experience experience)
    {
        player.GainCredits(credits);
        player.GainExperience(experience);
    }
}
