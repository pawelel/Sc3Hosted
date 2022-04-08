using Sc3Hosted.Shared.Enumerations;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sc3Hosted.Shared.Dtos;
public class AssetUpdateDto
{
    public string Name { get; set; }= string.Empty;

    public Status Status { get; set; }
    public virtual List<AssetDetailDto> AssetDetails { get; set; } = new();
    /// <summary>
    /// Process based location
    /// </summary>
    public int CoordinateId { get; set; }
    public virtual List<AssetCategoryDto> AssetCategories { get; set; } = new();
    /// <summary>
    /// Process name
    /// </summary>
    public string Process { get; set; }= string.Empty;
}
