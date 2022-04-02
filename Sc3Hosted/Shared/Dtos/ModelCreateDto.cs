using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sc3Hosted.Shared.Dtos;
public class ModelCreateDto
{
    public string Name { get; set; }

    public ModelCreateDto(string name)
    {
        Name = name;
    }
}
