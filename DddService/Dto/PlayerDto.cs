using DddService.Aggregates.PlayerNamespace;

namespace DddService.Dto;

public record PlayerDto(string Id, string Nickname, int Credits, int Experience, string Rank, string CommandCenterId)
{
    public static PlayerDto From(Player player)
    {
        return new PlayerDto(
            player.Id.ToString(),
            player.Nickname.Value,
            player.Credits.Value,
            player.Experience.Value,
            player.Rank.Value,
            player.CommandCenterId.ToString()
        );
    }
};
public record PlayerInputModel(string Nickname);
