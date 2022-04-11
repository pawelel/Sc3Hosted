namespace Sc3Hosted.Server.Entities;
public class CommunicateDevice : BaseEntity
{

    public int DeviceId { get; set; }
    public virtual Device Device { get; set; } = new();
    public int CommunicateId { get; set; }
    public virtual Communicate Communicate { get; set; } = new();
}
