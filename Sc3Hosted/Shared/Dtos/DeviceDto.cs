

namespace Sc3Hosted.Shared.Dtos;
public class DeviceDto
{
    public int DeviceId { get; set; }
    public string? Name { get; set; }

    public List<ModelDto> Models { get; set; } 
    public List<DeviceCommunicateDto> DeviceCommunicates { get; set; }
}