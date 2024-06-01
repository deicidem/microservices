using DddService.Aggregates;
using DddService.Aggregates.MissionNamespace;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DddService.Features.CreatingMission;

public record CreateMissionCommand(string CommandCenterId, string MissionTypeId, string PlanetId, Difficulty Difficulty) : IRequest<CreateMissionResult>
{
}

public record CreateMissionResult(Guid Id);

public class CreateMissionCommandHandler : IRequestHandler<CreateMissionCommand, CreateMissionResult>
{
    private readonly HelldiversDbContext _db;

    public CreateMissionCommandHandler(HelldiversDbContext db)
    {
        _db = db;
    }

    public async Task<CreateMissionResult> Handle(CreateMissionCommand request, CancellationToken cancellationToken)
    {
        var missionType = await _db.MissionTypes.FirstOrDefaultAsync(p => p.Id == Guid.Parse(request.MissionTypeId));
        if (missionType is null)
        {
            throw new Exception();
        }

        var commandCenter = await _db.CommandCenters.Include(c => c.Player).FirstOrDefaultAsync(p => p.Id == Guid.Parse(request.CommandCenterId));
        if (commandCenter is null)
        {
            throw new Exception();
        }

        var planet = await _db.Planets.FirstOrDefaultAsync(p => p.Id == Guid.Parse(request.PlanetId));
        if (planet is null)
        {
            throw new Exception();
        }


        var mission = commandCenter.InitiateMission(missionType, request.Difficulty, planet);

        await _db.SaveChangesAsync();

        return new CreateMissionResult(mission.Id);

    }
}