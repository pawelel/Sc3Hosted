using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sc3Hosted.Shared.Dtos;
public class AssetWithSituationsAndCategoriesDto
{
    public int AssetId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string DeviceName { get; set; } = string.Empty;
    public string ModelName { get; set; } = string.Empty;
    public List<CategoryDto> Categories { get; set; } = new();
    public List<SituationDto> Situations { get; set; } = new();
}
