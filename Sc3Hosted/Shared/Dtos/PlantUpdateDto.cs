using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sc3Hosted.Shared.Dtos;
public class PlantUpdateDto
{
    public string Name { get; set; }
    public List<AreaDto> Areas { get; set; }
    public PlantUpdateDto(string name)
    {
        Name = name;
    }
}
