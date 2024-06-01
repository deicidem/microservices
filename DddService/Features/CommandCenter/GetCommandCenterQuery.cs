
using DddService.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DddService.Features.CommandCenterFeature;
public record GetCommandCenterByIdQuery(string playerId) : IRequest<CommandCenterDto>;


public class GetCommandCenterByIdQueryHandler : IRequestHandler<GetCommandCenterByIdQuery, CommandCenterDto>
{
    private readonly HelldiversDbContext _db;

    public GetCommandCenterByIdQueryHandler(HelldiversDbContext db)
    {
        _db = db;
    }

    public async Task<CommandCenterDto> Handle(GetCommandCenterByIdQuery request, CancellationToken cancellationToken)
    {
        var commandCenter = await _db.CommandCenters.FirstOrDefaultAsync(p => p.PlayerId == Guid.Parse(request.playerId), cancellationToken: cancellationToken);
        if (commandCenter is null)
        {
            throw new Exception();
        }

        return CommandCenterDto.From(commandCenter);
    }
}