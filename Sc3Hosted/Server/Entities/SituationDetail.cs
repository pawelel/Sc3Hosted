namespace Sc3Hosted.Server.Entities;

public class SituationDetail : BaseEntity
{
    public int SituationDetailId { get; set; }
    public int SituationId { get; set; }
    public int DetailId { get; set; }
    public virtual Situation Situation { get; set; } = new();
    public virtual Detail Detail { get; set; } = new();
}