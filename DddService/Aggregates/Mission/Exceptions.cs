using DddService.Common;

namespace DddService.Aggregates.MissionNamespace;

public class InvalidSpaceImageIdException : BadRequestException
{
    public InvalidSpaceImageIdException(Guid spaceImageId)
        : base($"spaceImageId: '{spaceImageId}' is invalid.")
    {
    }
}


public class InvalidNameException : BadRequestException
{
    public InvalidNameException() : base("Name cannot be empty or whitespace.")
    {
    }
}
public class InvalidDateException : BadRequestException
{
    public InvalidDateException() : base("Date is invalid.")
    {
    }
}
public class InvalidGeographyException : BadRequestException
{
    public InvalidGeographyException() : base("Geography is not a valid JSON")
    {
    }
}
public class InvalidSatelliteException : BadRequestException
{
    public InvalidSatelliteException() : base("Satellite cannot be empty or whitespace.")
    {
    }
}
public class InvalidCloudCoverException : BadRequestException
{
    public InvalidCloudCoverException() : base("CloudCover must be between 0 and 100")
    {
    }
}
public class InvalidPreviewLinkException : BadRequestException
{
    public InvalidPreviewLinkException() : base("PreviewLink must be a valid URL and end with .png")
    {
    }
}
public class InvalidDirectoryLinkException : BadRequestException
{
    public InvalidDirectoryLinkException() : base("DirectoryLink must be a valid URL")
    {
    }
}
