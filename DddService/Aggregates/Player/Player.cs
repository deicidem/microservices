using System.Collections;
using System.ComponentModel.DataAnnotations.Schema;
using DddService.Aggregates.CommandCenterNamespace;
using DddService.Common;

namespace DddService.Aggregates.PlayerNamespace;
public class Player : Aggregate<PlayerId>
{
    public Nickname Nickname { get; set; } = default!;
    public Rank Rank { get; set; } = default!;
    public Credits Credits { get; set; } = default!;
    public Experience Experience { get; set; } = default!;
    public CommandCenter? CommandCenter { get; set; }
    public CommandCenterId? CommandCenterId { get; set; }
    public static Player Create(Player player)
    {
        return new Player
        {
            Id = player.Id,
            Nickname = player.Nickname,
            Rank = player.Rank,
            Credits = player.Credits,
            Experience = player.Experience,
            CommandCenter = player.CommandCenter,
            CommandCenterId = player.CommandCenterId
        };
    }
    public static Player Create(PlayerId id, Nickname name, Rank rank, Credits credits, Experience experience, CommandCenter commandCenter, CommandCenterId commandCenterId, bool isDeleted = false)
    {
        return new Player
        {
            Id = id,
            Nickname = name,
            Rank = rank,
            Credits = credits,
            Experience = experience,
            CommandCenter = commandCenter,
            CommandCenterId = commandCenterId,
            IsDeleted = isDeleted
        };
    }
    public static Player Create(PlayerId id, Nickname name)
    {
        return new Player
        {
            Id = id,
            Nickname = name,
            Credits = Credits.Of(0),
            Experience = Experience.Of(0),
            Rank = Rank.Of(0)
        };
    }

    public void EarnCredits(Credits credits)
    {
        Credits = Credits.Of(Credits.Value + credits.Value);
    }
    public void SpendCredits(Credits credits)
    {
        if (credits.Value > Credits.Value)
        {
            throw new Exception();
        }

        Credits = Credits.Of(Credits.Value - credits.Value);
    }
    public void GainExperience(Experience experience)
    {
        Experience = Experience.Of(Experience.Value + experience.Value);
    }
    public void ConnectToCommandCenter(CommandCenter commandCenter)
    {
        CommandCenter = commandCenter;
    }
}
