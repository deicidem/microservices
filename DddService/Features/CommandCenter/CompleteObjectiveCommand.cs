using DddService.Aggregates;
using DddService.Aggregates.MissionNamespace;
using DddService.Database;
using DddService.Dto;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DddService.Features.CommandCenterFeature;

public record CompleteObjectiveCommand(string CommandCenterId) : IRequest
{
}


public class CompleteObjectiveCommandHandler : IRequestHandler<CompleteObjectiveCommand>
{
    private readonly HelldiversDbContext _db;

    public CompleteObjectiveCommandHandler(HelldiversDbContext db)
    {
        _db = db;
    }

    public async Task Handle(CompleteObjectiveCommand request, CancellationToken cancellationToken)
    {

        var commandCenter = await _db.CommandCenters.Include(x => x.Player).Include(x => x.Mission).ThenInclude(x => x.Objectives)
            .FirstOrDefaultAsync(x => x.Id == Guid.Parse(request.CommandCenterId), cancellationToken: cancellationToken);
        if (commandCenter is null)
        {
            throw new Exception();
        }
        if (commandCenter.Mission is null)
        {
            throw new Exception();
        }

        commandCenter.Mission.CompleteObjective();

        await _db.SaveChangesAsync(cancellationToken);
    }
}