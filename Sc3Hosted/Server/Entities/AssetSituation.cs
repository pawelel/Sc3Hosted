namespace Sc3Hosted.Server.Entities;

public class AssetSituation : BaseEntity
{
  
    public int AssetId { get; set; } 
    public int SituationId { get; set; } 
    public virtual Asset Asset { get; set; } = new();
    public virtual Situation Situation { get; set; } = new();
}

