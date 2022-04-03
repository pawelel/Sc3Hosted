namespace Sc3Hosted.Server.Entities;
public class Detail : BaseEntity
{
    public int DetailId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public virtual List<SituationDetail> SituationDetails { get; set; } = new();
    public virtual List<AssetDetail> AssetDetails { get; set; } = new();
}
