
using DddService.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DddService.Features.PlayerFeature;
public record GetAllPlayersQuery : IRequest<IList<PlayerDto>>;


public class GetAllPlayersQueryHandler : IRequestHandler<GetAllPlayersQuery, IList<PlayerDto>>
{
    private readonly HelldiversDbContext _db;

    public GetAllPlayersQueryHandler(HelldiversDbContext db)
    {
        _db = db;
    }

    public async Task<IList<PlayerDto>> Handle(GetAllPlayersQuery request, CancellationToken cancellationToken)
    {
        return await _db.Players.Select(p => PlayerDto.From(p)).ToListAsync(cancellationToken: cancellationToken);
    }
}