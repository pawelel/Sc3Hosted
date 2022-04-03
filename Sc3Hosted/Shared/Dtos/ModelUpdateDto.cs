using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sc3Hosted.Shared.Dtos;
public class ModelUpdateDto
{
    public string Name { get; set; }= string.Empty;
    
    public List<ModelParameterDto> ModelParameters { get; set; }=new();
    public List<AssetDto> Assets { get; set; }=new();
}
