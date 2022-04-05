

namespace Sc3Hosted.Shared.Dtos;
public class DeviceDto :BaseDto
{
    public int DeviceId { get; set; }
    public string Name { get; set; } = string.Empty;
 public string Description { get; set; } = string.Empty;
    public List<ModelDto> Models { get; set; } = new();
    public List<CommunicateDeviceDto> CommunicateDevices { get; set; } = new();
    public List<DeviceSituationDto> DeviceSituations { get; set; } = new();
}