using System.Collections;
using DddService.Aggregates.MissionNamespace;
using DddService.Aggregates.PlayerNamespace;
using DddService.Common;

namespace DddService.Aggregates.CommandCenterNamespace;

public class CommandCenter : Aggregate
{
    public virtual Player Player { get; set; } = default!;
    public Guid PlayerId { get; set; } = default!;
    public Mission? Mission { get; set; } = default!;
    public Guid? MissionId { get; set; } = default!;
    public Difficulty HighestDifficultyAvailable { get; set; } = default!;
    public static CommandCenter Create(Guid id, Player player)
    {
        return new CommandCenter
        {
            Id = id,
            Player = player,
            HighestDifficultyAvailable = Difficulty.Easy,
            IsDeleted = false
        };
    }
    public Mission InitiateMission(MissionType missionType, Difficulty difficulty, Planet planet)
    {
        if (MissionId != null)
        {
            throw new Exception();
        }

        var mission = Mission.Create(
            Guid.NewGuid(),
            missionType,
            Player,
            difficulty,
            planet
        );
        Mission = mission;
        return mission;
    }

    public void StartMission()
    {
        if (Mission == null)
        {
            throw new Exception();
        }
        if (!Player.Id.Equals(Mission.InitiatorId))
        {
            throw new Exception();
        }

        Mission.Start();
    }

    public void AbandonMission()
    {
        if (Mission == null)
        {
            throw new Exception();
        }

        Mission.RemoveFromSquad(Player);
        MissionId = null;
    }

    public Mission SearchForMission(IEnumerable<Mission> missions)
    {
        if (Mission != null)
        {
            throw new Exception();
        }

        var mission = missions.Where(m => m.Status == MissionStatus.Initiated ||
                m.Status == MissionStatus.InProgress &&
                m.Difficulty <= HighestDifficultyAvailable &&
                m.Squad.GetPlayers().Count < 4)
            .FirstOrDefault();

        if (mission == null)
        {
            throw new Exception();
        }

        mission.AddToSquad(Player);
        Mission = mission;

        return mission;
    }

    public void RewardPlayer(Credits credits, Experience experience, Difficulty difficulty)
    {
        if (MissionId != null)
        {
            MissionId = null;
        }
        Player.EarnCredits(credits);
        Player.GainExperience(experience);
        UpdateHighestDifficultyAvailable(difficulty);
    }

    public void UpdateHighestDifficultyAvailable(Difficulty difficulty)
    {
        if (difficulty == HighestDifficultyAvailable && difficulty != Difficulty.Hell)
        {
            HighestDifficultyAvailable = difficulty + 1;
        }
    }


}
