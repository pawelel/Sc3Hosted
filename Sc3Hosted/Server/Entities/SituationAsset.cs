namespace Sc3Hosted.Server.Entities;

public class SituationAsset 
{
    public int SituationAssetId { get; set; }
    public int SituationId { get; set; }
    public int AssetId { get; set; }
    public Situation? Situation { get; set; } 
    public Asset? Asset { get; set; } 
}