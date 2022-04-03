using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sc3Hosted.Shared.Dtos;
public class DeviceUpdateDto
{
    public string Name { get; set; }

    public DeviceUpdateDto(string name)
    {
        Name = name;
    }
    public List<ModelDto> Models { get; set; }
    public List<DeviceCommunicateDto> DeviceCommunicates { get; set; }
}
