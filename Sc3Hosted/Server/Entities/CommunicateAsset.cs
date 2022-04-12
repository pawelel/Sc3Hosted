namespace Sc3Hosted.Server.Entities;
public class CommunicateAsset : BaseEntity
{

    public int AssetId { get; set; }
    public virtual Asset Asset { get; set; } = new();
    public int CommunicateId { get; set; }
    public virtual Communicate Communicate { get; set; } = new();
}
