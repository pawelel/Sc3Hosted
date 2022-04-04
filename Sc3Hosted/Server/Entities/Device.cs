namespace Sc3Hosted.Server.Entities;
public class Device : BaseEntity
{
    public int DeviceId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public virtual List<Model> Models { get; set; } = new();
    public virtual List<CommunicateDevice> DeviceCommunicates { get; set; } = new();
}