using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sc3Hosted.Shared.Dtos;
public class CoordinateCreateDto
{
    public string Name { get; set; }

    public CoordinateCreateDto(string name)
    {
        Name = name;
    }

    public string? Description { get; set; }
}
