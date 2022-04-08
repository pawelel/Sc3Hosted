namespace Sc3Hosted.Shared.Dtos;

public class CoordinateDto : BaseDto
{
    public int CoordinateId { get; set; }
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public virtual List<CommunicateCoordinateDto> CommunicateCoordinates { get; set; } = new();
    public virtual List<AssetDto> Assets { get; set; } = new();
}