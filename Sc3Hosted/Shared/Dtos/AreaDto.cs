namespace Sc3Hosted.Shared.Dtos;

public class AreaDto : BaseDto
{
    public int AreaId { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<SpaceDto> Spaces { get; set; } = new();
    public string Description { get; set; } = string.Empty;

    public List<CommunicateAreaDto> CommunicateAreas { get; set; } = new();
}