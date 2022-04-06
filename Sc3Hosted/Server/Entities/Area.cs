
namespace Sc3Hosted.Server.Entities;

public class Area : BaseEntity
{
    public int AreaId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int PlantId { get; set; }
    public virtual Plant Plant { get; set; } = new();

    public virtual List<Space> Spaces { get; set; } = new();
    public virtual List<CommunicateArea> CommunicateAreas { get; set; } = new();
}
