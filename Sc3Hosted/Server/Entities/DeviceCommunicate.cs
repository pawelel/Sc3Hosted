namespace Sc3Hosted.Server.Entities;
public class DeviceCommunicate
{
    public int DeviceCommunicateId { get; set; }
    public int DeviceId { get; set; }
    public Device? Device { get; set; } 
    public int CommunicateId { get; set; }
    public Communicate? Communicate { get; set; } 
}
