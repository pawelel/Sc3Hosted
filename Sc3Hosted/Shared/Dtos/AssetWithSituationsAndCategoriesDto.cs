namespace Sc3Hosted.Shared.Dtos;
public class AssetWithSituationsAndCategoriesDto
{
    public int AssetId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string DeviceName { get; set; } = string.Empty;
    public string ModelName { get; set; } = string.Empty;
    public virtual List<CategoryDto> Categories { get; set; } = new();
    public virtual List<SituationDto> Situations { get; set; } = new();
}
