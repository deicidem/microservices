using DddService.Common;

namespace DddService.Aggregates.SpaceImage;

// Entity - Aggregates
public class SpaceImage : Aggregate<SpaceImageId>
{
    public Name Name { get; private set; } = default!;
    public Date? Date { get; private set; }
    public Geography? Geography { get; private set; }
    public Satellite? Satellite { get; private set; }
    public CloudCover? CloudCover { get; private set; }
    public Status Status { get; private set; } = default!;
    public PreviewLink? PreviewLink { get; private set; }
    public DirectoryLink DirectoryLink { get; private set; } = default!;

    public static SpaceImage Create(SpaceImageId id, Name name, Date date, Geography geography, Satellite satellite, CloudCover cloudCover, Status status, PreviewLink previewLink, DirectoryLink directoryLink, bool isDeleted = false)
    {
        var image = new SpaceImage
        {
            Id = id,
            Name = name,
            Date = date,
            Geography = geography,
            Satellite = satellite,
            CloudCover = cloudCover,
            Status = status,
            PreviewLink = previewLink,
            DirectoryLink = directoryLink
        };

        return image;
    }

    public static SpaceImage Create(SpaceImageId id, DirectoryLink directoryLink, bool isDeleted = false)
    {
        var image = new SpaceImage
        {
            Id = id,
            Name = Name.Of("unknown image"),
            Status = Status.Of(AvailableStatuses.Uploaded),
            DirectoryLink = directoryLink
        };

        // image processing
        // .
        // .
        // .

        image.UpdateName("Processed image");
        image.UpdateCloudCover(CloudCover.Of(10));
        image.UpdateDate(Date.Of(DateTime.Now));
        image.UpdateGeography(Geography.Of("{\"type\":\"Polygon\",\"coordinates\":[[[-180,90],[180,90],[180,-90],[-180,-90],[-180,90]]]}"));
        image.UpdateSatellite(Satellite.Of("Sentinel 2A"));
        image.UpdateStatus(Status.Of(AvailableStatuses.Available));
        image.UpdatePreviewLink(PreviewLink.Of("https://example.com/image.png"));


        return image;
    }

    public void UpdateName(Name name)
    {
        Name = name;
    }
    public void UpdateDate(Date date)
    {
        Date = date;
    }
    public void UpdateGeography(Geography geography)
    {
        Geography = geography;
    }
    public void UpdateSatellite(Satellite satellite)
    {
        Satellite = satellite;
    }
    public void UpdateCloudCover(CloudCover cloudCover)
    {
        CloudCover = cloudCover;
    }
    public void UpdateStatus(Status status)
    {
        Status = status;
    }
    public void UpdatePreviewLink(PreviewLink previewLink)
    {
        PreviewLink = previewLink;
    }
    public void UpdateDirectoryLink(DirectoryLink directoryLink)
    {
        DirectoryLink = directoryLink;
    }

}
