using DddService.Aggregates;
using DddService.Aggregates.CommandCenterNamespace;
using DddService.Aggregates.PlayerNamespace;
using DddService.Database;
using DddService.Dto;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DddService.Features.PlayerFeature;

public record CreatePlayerCommand(string Nickname) : IRequest<PlayerDto>
{
}


public class CreatePlayerCommandHandler : IRequestHandler<CreatePlayerCommand, PlayerDto>
{
    private readonly HelldiversDbContext _db;

    public CreatePlayerCommandHandler(HelldiversDbContext db)
    {
        _db = db;
    }

    public async Task<PlayerDto> Handle(CreatePlayerCommand request, CancellationToken cancellationToken)
    {
        var player = await _db.Players.SingleOrDefaultAsync(x => x.Nickname.Value == request.Nickname, cancellationToken: cancellationToken);
        if (player is not null)
        {
            throw new Exception();
        }

        var newPlayer = Player.Create(
            Guid.NewGuid(),
            Nickname.Of(request.Nickname)
        );
        var PlayerEntity = _db.Players.Add(newPlayer).Entity;
        await _db.SaveChangesAsync(cancellationToken);

        var newCommandCenter = CommandCenter.Create(
            Guid.NewGuid(),
            newPlayer
        );
        newCommandCenter = _db.CommandCenters.Add(newCommandCenter).Entity;
        await _db.SaveChangesAsync(cancellationToken);


        PlayerEntity.ConnectToCommandCenter(newCommandCenter);

        await _db.SaveChangesAsync(cancellationToken);

        return PlayerDto.From(PlayerEntity);
    }
}