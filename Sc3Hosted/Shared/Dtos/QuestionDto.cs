namespace Sc3Hosted.Shared.Dtos;
/// <summary>
///     Trouble shooting question not from DB
/// </summary>
public class QuestionDto : BaseDto
{
    public int QuestionId { get; set; }
    public string Name { get; set; } = string.Empty;
    public virtual List<SituationQuestionDto> SituationQuestions { get; set; } = new();
}
