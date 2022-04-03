namespace Sc3Hosted.Server.Entities;
public class SpaceCommunicate : BaseEntity
{
    public int SpaceCommunicateId { get; set; }
    public int SpaceId { get; set; }
    public virtual Space Space { get; set; } = new();
    public int CommunicateId { get; set; }
    public virtual Communicate Communicate { get; set; } = new();
}
