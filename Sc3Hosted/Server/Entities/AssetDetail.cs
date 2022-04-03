namespace Sc3Hosted.Server.Entities;

public class AssetDetail
{
    public int AssetDetailId { get; set; }
    public int AssetId { get; set; }
    public int DetailId { get; set; }
    public virtual Asset? Asset { get; set; }
    public virtual Detail? Detail { get; set; }
    public string? Value { get; set; }
}