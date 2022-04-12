namespace Sc3Hosted.Server.Entities;
public class CategorySituation : BaseEntity
{


    public int CategoryId { get; set; }
    public virtual Situation Situation { get; set; } = new();
    public int SituationId { get; set; }
    public virtual Category Category { get; set; } = new();
}
