using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sc3Hosted.Shared.Dtos;
public class ModelUpdateDto
{
    public string Name { get; set; }
    public ModelUpdateDto(string name)
    {
        Name = name;
    }
    public List<ModelParameterDto> ModelParameters { get; set; }
    public List<AssetDto> Assets { get; set; }
}
