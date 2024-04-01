namespace DddService.Dto;

public record AirportDto(string Id, string Name, string Address, string Code);
public record AirportInputModel(string Name, string Address, string Code);