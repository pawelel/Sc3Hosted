namespace Sc3Hosted.Server.Entities;

public class Coordinate 
{
    public int CoordinateId { get; set; }
    public string Name { get; set; }

    public Coordinate(string name)
    {
        Name = name;
    }

    public string? Description { get; set; }
    public Space? Space { get; set; } 
    public int SpaceId { get; set; }
    public bool IsArchived { get; set; }
    public List<CoordinateCommunicate>? CoordinateCommunicates { get; set; } 
}
