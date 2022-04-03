namespace Sc3Hosted.Shared.Dtos;

public class AreaDto
{
    public int AreaId { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<SpaceDto> Spaces { get; set; } = new();
    public string Description { get; set; } = string.Empty;

    public List<AreaCommunicateDto> AreaCommunicates { get; set; } = new();
}