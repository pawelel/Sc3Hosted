using Sc3Hosted.Shared.Enumerations;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sc3Hosted.Shared.Dtos;
public class SpaceCreateDto
{
    public string Name { get; set; }= string.Empty;
public string Description { get; set; }=string.Empty;
    public SpaceType SpaceType { get; set; }
    public bool IsArchived { get; set; }
}
