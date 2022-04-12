namespace Sc3Hosted.Server.Entities;
public class AssetCategory : BaseEntity
{
    public int AssetId { get; set; }
    public virtual Asset Asset { get; set; } = new();
    public int CategoryId { get; set; }
    public virtual Category Category { get; set; } = new();
}
