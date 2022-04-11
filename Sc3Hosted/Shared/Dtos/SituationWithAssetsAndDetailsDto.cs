namespace Sc3Hosted.Shared.Dtos;
public class SituationWithAssetsAndDetailsDto : BaseDto
    {
    public int SituationId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public virtual List<AssetWithDetailsDisplayDto> Assets { get; set; } = new();
    }
