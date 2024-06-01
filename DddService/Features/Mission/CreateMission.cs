using DddService.Aggregates;
using DddService.Aggregates.MissionNamespace;
using DddService.Dto;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DddService.Features.MissionFeature;

public record CreateMissionCommand(string CommandCenterId, string MissionTypeId, string PlanetId, Difficulty Difficulty) : IRequest<MissionDto>
{
}


public class CreateMissionCommandHandler : IRequestHandler<CreateMissionCommand, MissionDto>
{
    private readonly HelldiversDbContext _db;

    public CreateMissionCommandHandler(HelldiversDbContext db)
    {
        _db = db;
    }

    public async Task<MissionDto> Handle(CreateMissionCommand request, CancellationToken cancellationToken)
    {
        var missionType = await _db.MissionTypes.FirstOrDefaultAsync(p => p.Id == Guid.Parse(request.MissionTypeId), cancellationToken: cancellationToken);
        if (missionType is null)
        {
            throw new Exception();
        }

        var commandCenter = await _db.CommandCenters.Include(c => c.Player).FirstOrDefaultAsync(p => p.Id == Guid.Parse(request.CommandCenterId), cancellationToken: cancellationToken);
        if (commandCenter is null)
        {
            throw new Exception();
        }

        var planet = await _db.Planets.FirstOrDefaultAsync(p => p.Id == Guid.Parse(request.PlanetId), cancellationToken: cancellationToken);
        if (planet is null)
        {
            throw new Exception();
        }


        var mission = commandCenter.InitiateMission(missionType, request.Difficulty, planet);
        await _db.SaveChangesAsync(cancellationToken);

        return MissionDto.From(mission);

    }
}