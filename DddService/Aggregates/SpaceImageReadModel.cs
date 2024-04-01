namespace DddService.Aggregates.SpaceImage;

public class SpaceImageReadModel
{
    public required Guid Id { get; init; }
    public required Guid AirportId { get; init; }
    public string? Name { get; init; }
    public string? Geography { get; init; }
    public string? Satellite { get; init; }
    public string? Date { get; init; }
    public float? CloudCover { get; init; }
    public required string Status { get; init; }
    public string? PreviewLink { get; init; }

    public required string DirectoryLink { get; init; }
    public required bool IsDeleted { get; init; }
}
