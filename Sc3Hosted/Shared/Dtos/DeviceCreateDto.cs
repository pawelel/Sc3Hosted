using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sc3Hosted.Shared.Dtos;
public class DeviceCreateDto
{
    public int DeviceId { get; set; }
    public string Name { get; set; }

    public DeviceCreateDto(string name)
    {
        Name = name;
    }
}
