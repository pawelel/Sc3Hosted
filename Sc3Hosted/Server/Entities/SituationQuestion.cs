namespace Sc3Hosted.Server.Entities;
public class SituationQuestion : BaseEntity
{
    public int SituationId { get; set; }
    public virtual Situation Situation { get; set; } = new();
    public int QuestionId { get; set; }
    public virtual Question Question { get; set; } = new();
}
