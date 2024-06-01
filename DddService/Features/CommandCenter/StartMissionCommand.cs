using DddService.Aggregates;
using DddService.Aggregates.MissionNamespace;
using DddService.Database;
using DddService.Dto;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DddService.Features.CommandCenterFeature;

public record StartMissionCommand(string CommandCenterId) : IRequest<MissionDto>
{
}


public class StartMissionCommandHandler : IRequestHandler<StartMissionCommand, MissionDto>
{
    private readonly HelldiversDbContext _db;

    public StartMissionCommandHandler(HelldiversDbContext db)
    {
        _db = db;
    }

    public async Task<MissionDto> Handle(StartMissionCommand request, CancellationToken cancellationToken)
    {

        var commandCenter = await _db.CommandCenters.Include(x => x.Player).Include(x => x.Mission)
            .FirstOrDefaultAsync(x => x.Id == Guid.Parse(request.CommandCenterId), cancellationToken: cancellationToken);
        if (commandCenter is null)
        {
            throw new Exception();
        }

        var missions = await _db.Missions.ToListAsync(cancellationToken: cancellationToken);
        commandCenter.StartMission();

        await _db.SaveChangesAsync(cancellationToken);
        var mission = await _db.Missions.Include(x => x.Planet).Include(x => x.Type).Include(x => x.Objectives).FirstAsync(x => x.Id == commandCenter.MissionId, cancellationToken: cancellationToken);
        return MissionDto.From(mission);
    }
}