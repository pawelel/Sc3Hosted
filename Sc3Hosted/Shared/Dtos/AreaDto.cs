namespace Sc3Hosted.Shared.Dtos;

public class AreaDto
{
    public int AreaId { get; set; }
    public string? Name { get; set; }
    public List<SpaceDto> Spaces { get; set; } 
    public List<AreaCommunicateDto> AreaCommunicates { get; set; } 
}