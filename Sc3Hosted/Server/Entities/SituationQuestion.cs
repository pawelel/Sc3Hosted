
namespace Sc3Hosted.Server.Entities;

public class SituationQuestion 
{
    public int SituationQuestionId { get; set; }
    public int SituationId { get; set; }
    public Situation? Situation { get; set; } 
    public int QuestionId { get; set; }
    public Question? Question { get; set; } 
}