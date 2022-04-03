using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sc3Hosted.Shared.Dtos;
public class CoordinateUpdateDto
{
    public string Name { get; set; }= string.Empty;

    public string Description { get; set; } = string.Empty;
    public bool IsArchived { get; set; }
    public List<CoordinateCommunicateDto> CoordinateCommunicates { get; set; }= new();
}
