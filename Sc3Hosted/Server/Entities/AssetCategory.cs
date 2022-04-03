namespace Sc3Hosted.Server.Entities;
public class AssetCategory
{
    public int AssetCategoryId { get; set; }
    public int AssetId { get; set; }
    public Asset? Asset { get; set; }
    public int CategoryId { get; set; }
    public Category? Category { get; set; }
}
