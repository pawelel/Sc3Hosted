namespace Sc3Hosted.Shared.Dtos;
public class SituationWithQuestionsDto : BaseDto
{
    public int SituationId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public virtual List<QuestionDto> Questions { get; set; } = new();
}
