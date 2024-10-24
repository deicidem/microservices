using DddService.Aggregates;
using DddService.Aggregates.MissionNamespace;
using DddService.Database;
using DddService.Dto;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DddService.Features.CommandCenterFeature;

public record AbandonMissionCommand(string CommandCenterId) : IRequest
{
}


public class AbandonMissionCommandHandler : IRequestHandler<AbandonMissionCommand>
{
    private readonly HelldiversDbContext _db;

    public AbandonMissionCommandHandler(HelldiversDbContext db)
    {
        _db = db;
    }

    public async Task Handle(AbandonMissionCommand request, CancellationToken cancellationToken)
    {

        var commandCenter = await _db.CommandCenters.Include(x => x.Player).Include(x => x.Mission).ThenInclude(x => x.Objectives)
            .FirstOrDefaultAsync(x => x.Id == Guid.Parse(request.CommandCenterId), cancellationToken: cancellationToken);
        if (commandCenter is null)
        {
            throw new Exception();
        }

        commandCenter.AbandonMission();

        await _db.SaveChangesAsync(cancellationToken);
    }
}