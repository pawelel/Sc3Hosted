namespace Sc3Hosted.Shared.Dtos;

public class CoordinateDto
{
    public int CoordinateId { get; set; }
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
    public bool IsArchived { get; set; }
    public List<CoordinateCommunicateDto> CoordinateCommunicates { get; set; } = new();
}