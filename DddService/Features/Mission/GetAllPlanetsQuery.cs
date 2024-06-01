
using DddService.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Extensions;

namespace DddService.Features.MissionFeature;
public record GetAllPlanetsQuery : IRequest<IList<PlanetDto>>;


public class GetAllPlanetsQueryHandler : IRequestHandler<GetAllPlanetsQuery, IList<PlanetDto>>
{
    private readonly HelldiversDbContext _db;

    public GetAllPlanetsQueryHandler(HelldiversDbContext db)
    {
        _db = db;
    }

    public async Task<IList<PlanetDto>> Handle(GetAllPlanetsQuery request, CancellationToken cancellationToken)
    {
        return await _db.Planets.Select(p => PlanetDto.From(p)).ToListAsync();
    }
}