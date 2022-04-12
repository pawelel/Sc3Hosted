namespace Sc3Hosted.Shared.Dtos;
public class CoordinateFlat : BaseDto
{
    public int CoordinateId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
