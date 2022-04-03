namespace Sc3Hosted.Shared.Dtos;
public class SituationDto
{
    public int SituationId { get; set; }
    public string? Name { get; set; }

    public List<SituationQuestionDto> SituationQuestions { get; set; }
    public List<SituationDetailDto> SituationDetails { get; set; }
    public List<SituationParameterDto> SituationParameters { get; set; }
    public List<SituationCategoryDto> CategorySituations { get; set; }
    public List<SituationAssetDto> AssetSituations { get; set; }
}
