namespace Sc3Hosted.Shared.Dtos;
public class SituationWithCategoriesDto : BaseDto
{
    public int SituationId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public virtual List<CategoryDto> Categories { get; set; } = new();
}
