using DddService.Aggregates;
using DddService.Aggregates.MissionNamespace;
using DddService.Dto;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DddService.Features.MissionFeature;

public record SearchForMissionCommand(string CommandCenterId) : IRequest<MissionDto>
{
}


public class SearchForMissionCommandHandler : IRequestHandler<SearchForMissionCommand, MissionDto>
{
    private readonly HelldiversDbContext _db;

    public SearchForMissionCommandHandler(HelldiversDbContext db)
    {
        _db = db;
    }

    public async Task<MissionDto> Handle(SearchForMissionCommand request, CancellationToken cancellationToken)
    {

        var commandCenter = await _db.CommandCenters.Include(x => x.Player)
            .FirstOrDefaultAsync(x => x.Id == Guid.Parse(request.CommandCenterId), cancellationToken: cancellationToken);
        if (commandCenter is null)
        {
            throw new Exception();
        }

        var missions = await _db.Missions.ToListAsync(cancellationToken: cancellationToken);
        var mission = commandCenter.SearchForMission(missions);

        await _db.SaveChangesAsync(cancellationToken);
        mission = await _db.Missions.Include(x => x.Planet).Include(x => x.Type).Include(x => x.Objectives).FirstOrDefaultAsync(x => x.Id == mission.Id, cancellationToken: cancellationToken);
        return MissionDto.From(mission);
    }
}