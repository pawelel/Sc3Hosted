namespace Sc3Hosted.Server.Entities;

public class SituationAsset  : BaseEntity
{
    public int SituationAssetId { get; set; }
    public int SituationId { get; set; }
    public int AssetId { get; set; }
    public virtual Situation Situation { get; set; } = new();
    public virtual Asset Asset { get; set; } = new();
}