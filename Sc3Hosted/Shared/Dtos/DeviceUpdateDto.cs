using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sc3Hosted.Shared.Dtos;
public class DeviceUpdateDto
{
    public string Name { get; set; }= string.Empty;
 public string Description { get; set; } = string.Empty;
    public List<ModelDto> Models { get; set; }= new();
    public List<DeviceCommunicateDto> DeviceCommunicates { get; set; }= new();
}
