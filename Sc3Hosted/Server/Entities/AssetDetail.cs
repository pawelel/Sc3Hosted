namespace Sc3Hosted.Server.Entities;
public class AssetDetail : BaseEntity
{

    public int AssetId { get; set; }
    public int DetailId { get; set; }
    public virtual Asset Asset { get; set; } = new();
    public virtual Detail Detail { get; set; } = new();
    public string Value { get; set; } = string.Empty;
}
