using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sc3Hosted.Shared.Dtos;
public class ModelFlat
{
    public int ModelId { get; set; }
    public string? Name { get; set; }
    public List<ModelParameterDto>? ModelParameters { get; set; }

}
