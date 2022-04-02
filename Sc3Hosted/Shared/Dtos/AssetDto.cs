using Sc3Hosted.Shared.Enumerations;

namespace Sc3Hosted.Shared.Dtos;

public class AssetDto
{
    public int AssetId { get; set; }
    public string? Name { get; set; }
    public Status Status { get; set; }
    public List<AssetDetailDto>? AssetDetails { get; set; }
    public List<AssetCategoryDto>? AssetCategories { get; set; }
    public int CoordinateId { get; set; }
    public string? Process { get; set; }
}