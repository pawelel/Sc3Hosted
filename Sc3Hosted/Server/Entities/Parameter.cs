
namespace Sc3Hosted.Server.Entities;
public class Parameter  : BaseEntity
{
    public int ParameterId { get; set; }
    public string Name { get; set; }=string.Empty;

    public string Description { get; set; }=string.Empty;
    public virtual List<ModelParameter> ModelParameters { get; set; }=new();
    public virtual List<SituationParameter> SituationParameters { get; set; }=new();
}
