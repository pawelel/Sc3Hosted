namespace Sc3Hosted.Shared.Dtos;
public class DeviceUpdateDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public virtual List<ModelDto> Models { get; set; } = new();
    public virtual List<CommunicateDeviceDto> CommunicateDevices { get; set; } = new();
}
