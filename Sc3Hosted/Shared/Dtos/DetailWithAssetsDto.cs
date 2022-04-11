namespace Sc3Hosted.Shared.Dtos;

public class DetailWithAssetsDto : BaseDto
{
    public int DetailId { get; set; }
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
    public virtual List<AssetDto> Assets { get; set; } = new();
}