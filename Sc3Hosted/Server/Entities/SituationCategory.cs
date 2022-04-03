namespace Sc3Hosted.Server.Entities;

public class SituationCategory  : BaseEntity
{
    public int SituationCategoryId { get; set; }
    public int SituationId { get; set; }
    public int CategoryId { get; set; }
    public virtual Situation Situation { get; set; } = new();
    public virtual Category Category { get; set; } = new();
}