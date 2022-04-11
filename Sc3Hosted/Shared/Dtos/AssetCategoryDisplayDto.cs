namespace Sc3Hosted.Shared.Dtos;
public class AssetCategoryDisplayDto : BaseDto
{
    public string Name { get; set; }=string.Empty;
    public string Description { get; set; } = string.Empty;
    public int AssetId { get; set; }
    public int CategoryId { get; set; }
    
}
