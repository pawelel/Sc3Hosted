namespace Sc3Hosted.Server.Entities;
public class Situation : BaseEntity
{
    public int SituationId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public virtual List<SituationQuestion> SituationQuestions { get; set; } = new();
    public virtual List<SituationDetail> SituationDetails { get; set; } = new();
    public virtual List<SituationParameter> SituationParameters { get; set; } = new();
    public virtual List<CategorySituation> CategorySituations { get; set; } = new();
    public virtual List<DeviceSituation> DeviceSituations { get; set; } = new();
    public virtual List<AssetSituation> AssetSituations { get; set; } = new();
}
