namespace Sc3Hosted.Server.Entities;
public class Device 
{
    public int DeviceId { get; set; } 
    public string Name { get; set; }

    public Device(string name)
    {
        Name = name;
    }

    public List<Model>? Models { get; set; } 
    public List<DeviceCommunicate>? DeviceCommunicates { get; set; } 
}