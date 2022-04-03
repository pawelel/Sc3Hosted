namespace Sc3Hosted.Server.Entities;
public class AreaCommunicate : BaseEntity
{
    public int AreaCommunicateId { get; set; }
    public int AreaId { get; set; }
    public virtual Area Area { get; set; } =new();
    public int CommunicateId { get; set; }
    public virtual Communicate Communicate { get; set; } =new();
}
