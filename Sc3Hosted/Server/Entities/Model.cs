
namespace Sc3Hosted.Server.Entities;

public class Model : BaseEntity
{
    public int ModelId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int DeviceId { get; set; }
    public virtual Device Device { get; set; } = new();
    public virtual List<ModelParameter> ModelParameters { get; set; } = new();
    public virtual List<Asset> Assets { get; set; } = new();
}