namespace Sc3Hosted.Shared.Dtos;

public class SituationQuestionDto : BaseDto
{
    public int SituationQuestionId { get; set; }
    public int SituationId { get; set; }
    public int QuestionId { get; set; }
}