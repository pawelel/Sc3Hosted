namespace Sc3Hosted.Server.Entities;
public class ModelCommunicate : BaseEntity
{
    public int ModelCommunicateId { get; set; }
    public int ModelId { get; set; }
    public virtual Model Model { get; set; } = new();
    public int CommunicateId { get; set; }
    public virtual Communicate Communicate { get; set; } = new();
}
