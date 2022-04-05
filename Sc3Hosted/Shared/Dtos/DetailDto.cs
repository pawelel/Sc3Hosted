namespace Sc3Hosted.Shared.Dtos;
public class DetailDto : BaseDto
{
    public int DetailId { get; set; }
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
}
