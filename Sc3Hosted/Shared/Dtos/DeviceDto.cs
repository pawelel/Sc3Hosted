

namespace Sc3Hosted.Shared.Dtos;
public class DeviceDto
{
    public int DeviceId { get; set; }
    public string Name { get; set; } = string.Empty;
 public string Description { get; set; } = string.Empty;
    public List<ModelDto> Models { get; set; } = new();
    public List<DeviceCommunicateDto> DeviceCommunicates { get; set; } = new();
}