namespace Sc3Hosted.Shared.Dtos;
public class SituationDto : BaseDto
{
    public int SituationId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public virtual List<SituationQuestionDto> SituationQuestions { get; set; } = new();
    public virtual List<SituationDetailDto> SituationDetails { get; set; } = new();
    public virtual List<SituationParameterDto> SituationParameters { get; set; } = new();
    public virtual List<CategorySituationDto> CategorySituations { get; set; } = new();
    public virtual List<DeviceSituationDto> DeviceSituations { get; set; } = new();
    public  List<AssetSituationDto> AssetSituations { get; set; } = new();
    public virtual List<CommunicateCategoryDto> CommunicateCategories { get; set; } = new();
}
