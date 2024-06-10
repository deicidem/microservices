using DddService.Aggregates;
using DddService.Aggregates.MissionNamespace;
using DddService.Aggregates.PlayerNamespace;
using DddService.Database;
using DddService.Dto;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DddService.Features.CommandCenterFeature;

public record RewardPlayerCommand(string PlayerId, int Experience, int Credits, Difficulty Difficulty) : IRequest
{
}


public class RewardPlayerCommandHandler : IRequestHandler<RewardPlayerCommand>
{
    private readonly HelldiversDbContext _db;

    public RewardPlayerCommandHandler(HelldiversDbContext db)
    {
        _db = db;
    }

    public async Task Handle(RewardPlayerCommand request, CancellationToken cancellationToken)
    {

        var commandCenter = await _db.CommandCenters.Include(x => x.Player).Include(x => x.Mission)
            .FirstOrDefaultAsync(x => x.PlayerId == Guid.Parse(request.PlayerId), cancellationToken: cancellationToken);

        if (commandCenter is null)
        {
            throw new Exception();
        }

        commandCenter.RewardPlayer(Credits.Of(request.Credits), Experience.Of(request.Experience), Difficulty.Easy);

        await _db.SaveChangesAsync(cancellationToken);
    }
}