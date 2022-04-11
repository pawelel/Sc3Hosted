namespace Sc3Hosted.Server.Entities;
public class CommunicateArea : BaseEntity
{
  
    public int AreaId { get; set; }
    public virtual Area Area { get; set; } =new();
    public int CommunicateId { get; set; }
    public virtual Communicate Communicate { get; set; } =new();
}
