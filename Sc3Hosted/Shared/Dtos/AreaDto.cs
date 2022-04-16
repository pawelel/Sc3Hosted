namespace Sc3Hosted.Shared.Dtos;
public class AreaDto : BaseDto
{
    public int AreaId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int PlantId { get; set; }
    public virtual List<SpaceDto> Spaces { get; set; } = new();
    public virtual List<CommunicateAreaDto> CommunicateAreas { get; set; } = new();
}
