using Sc3Hosted.Shared.Enumerations;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sc3Hosted.Shared.Dtos;
public class SpaceUpdateDto
{
    public string Name { get; set; }= string.Empty;
public string Description { get; set; }=string.Empty;
    public List<CoordinateDto> Coordinates { get; set; }=new();
    public SpaceType SpaceType { get; set; }
    public bool IsArchived { get; set; }
    public List<SpaceCommunicateDto> SpaceCommunicates { get; set; }=new();
}
