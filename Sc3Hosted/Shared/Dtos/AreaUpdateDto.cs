namespace Sc3Hosted.Shared.Dtos;
public class AreaUpdateDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public virtual List<SpaceDto> Spaces { get; set; } = new();
    public virtual List<CommunicateAreaDto> CommunicateAreas { get; set; } = new();
}
