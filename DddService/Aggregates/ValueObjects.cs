using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace DddService.Aggregates.SpaceImage;

public class SpaceImageId
{
    public Guid Value { get; }

    private SpaceImageId(Guid value)
    {
        Value = value;
    }

    public static SpaceImageId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new InvalidAirportIdException(value);
        }

        return new SpaceImageId(value);
    }

    public static implicit operator Guid(SpaceImageId airportId)
    {
        return airportId.Value;
    }
}
public record Name
{
    public string Value { get; }

    private Name(string value)
    {
        Value = value;
    }

    public static Name Of(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidNameException();
        }

        return new Name(value);
    }

    public static implicit operator string(Name name)
    {
        return name.Value;
    }
}

public record Date
{
    public DateTime Value { get; }

    private Date(DateTime value)
    {
        Value = value;
    }

    public static Date Of(DateTime value)
    {
        if (value <= DateTime.MinValue || value >= DateTime.MaxValue)
        {
            throw new InvalidDateException();
        }

        return new Date(value);
    }

    public static implicit operator string(Date date)
    {
        return date.Value.ToUniversalTime().ToString("yyyy-MM-dd");
    }
}
public record Geography
{
    public string Value { get; }
    private Geography(string value)
    {
        Value = value;
    }

    public static Geography Of(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || !ValidateJson(value))
        {
            throw new InvalidGeographyException();
        }

        return new Geography(value);
    }

    private static bool ValidateJson(string json)
    {
        try
        {
            JsonNode.Parse(json);
            return true;
        }
        catch
        {
            return false;
        }
    }


    public static implicit operator string(Geography json)
    {
        return json.Value;
    }
}

public record Satellite
{
    public string Value { get; }

    private Satellite(string value)
    {
        Value = value;
    }

    public static Satellite Of(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidSatelliteException();
        }

        return new Satellite(value);
    }

    public static implicit operator string(Satellite satellite)
    {
        return satellite.Value;
    }
}
public record CloudCover
{
    public float Value { get; }

    private CloudCover(float value)
    {
        Value = value;
    }

    public static CloudCover Of(float value)
    {
        if (value < 0 || value > 100)
        {
            throw new InvalidCloudCoverException();
        }

        return new CloudCover(value);
    }

    public static implicit operator string(CloudCover cloudCover)
    {
        return cloudCover.Value.ToString();
    }
}
public enum AvailableStatuses
{
    Uploaded = 1,
    Queued = 2,
    Processing = 3,
    Error = 4,
    Available = 5
}
public record Status
{
    public AvailableStatuses Value { get; }

    private Status(AvailableStatuses value)
    {
        Value = value;
    }

    public static Status Of(AvailableStatuses value)
    {
        return new Status(value);
    }

    public static implicit operator string(Status status)
    {
        if (status.Value == AvailableStatuses.Available) return "Available";
        if (status.Value == AvailableStatuses.Error) return "Error";
        if (status.Value == AvailableStatuses.Processing) return "Processing";
        if (status.Value == AvailableStatuses.Queued) return "Queued";
        if (status.Value == AvailableStatuses.Uploaded) return "Uploaded";
        return "Unknown";
    }
}

public record PreviewLink
{
    public string Value { get; }

    private PreviewLink(string value)
    {
        Value = value;
    }

    public static PreviewLink Of(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || !value.EndsWith(".png"))
        {
            throw new InvalidPreviewLinkException();
        }
        return new PreviewLink(value);
    }

    public static implicit operator string(PreviewLink link)
    {
        return link.Value;
    }
}

public record DirectoryLink
{
    public string Value { get; }

    private DirectoryLink(string value)
    {
        Value = value;
    }

    public static DirectoryLink Of(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new Exception("Invalid directory_link");
        }
        return new DirectoryLink(value);
    }

    public static implicit operator string(DirectoryLink link)
    {
        return link.Value;
    }
}
