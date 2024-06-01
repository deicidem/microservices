namespace DddService.Dto;

public record PlayerDto(string Id, string Nickname, int Credits, int Experience, string Rank);
public record PlayerInputModel(string Nickname);
