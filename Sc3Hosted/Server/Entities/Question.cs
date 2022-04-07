namespace Sc3Hosted.Server.Entities;

public class Question : BaseEntity
{
    public int QuestionId { get; set; }
    public string Name { get; set; } = string.Empty;
    public virtual List<SituationQuestion> SituationQuestions { get; set; } = new();
}