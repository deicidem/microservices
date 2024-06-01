
using DddService.Database;
using DddService.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Extensions;

namespace DddService.Features.MissionFeature;
public record GetAllMissionTypesQuery : IRequest<IList<MissionTypeDto>>;


public class GetAllMissionTypesQueryHandler : IRequestHandler<GetAllMissionTypesQuery, IList<MissionTypeDto>>
{
    private readonly HelldiversDbContext _db;

    public GetAllMissionTypesQueryHandler(HelldiversDbContext db)
    {
        _db = db;
    }

    public async Task<IList<MissionTypeDto>> Handle(GetAllMissionTypesQuery request, CancellationToken cancellationToken)
    {
        return await _db.MissionTypes.Select(mt => MissionTypeDto.From(mt)).ToListAsync(cancellationToken: cancellationToken);
    }
}