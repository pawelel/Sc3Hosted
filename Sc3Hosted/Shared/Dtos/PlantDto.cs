namespace Sc3Hosted.Shared.Dtos;

public class PlantDto : BaseDto
{
    public int PlantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<AreaDto> Areas { get; set; } = new();
}