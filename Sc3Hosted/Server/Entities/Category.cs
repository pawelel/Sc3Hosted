namespace Sc3Hosted.Server.Entities;

public class Category : BaseEntity
{
    public int CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public virtual List<AssetCategory> AssetCategories { get; set; } = new();
    public virtual List<CategorySituation> CategorySituations { get; set; } = new();
    public virtual List<CommunicateCategory> CommunicateCategories { get; set; } = new();
}
