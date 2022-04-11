using Sc3Hosted.Shared.Enumerations;

namespace Sc3Hosted.Server.Entities;
public class Communicate  : BaseEntity
{
    public int CommunicateId { get; set; }
    public string Name { get; set; }=string.Empty;
    public string Description { get; set; } = string.Empty;
    public Scope Scope { get; set; }
    public CommunicationType CommunicationType { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public virtual List<CommunicateArea> CommunicateAreas { get; set; } = new();
    public virtual List<CommunicateAsset> CommunicateAssets { get; set; }  = new();
    public virtual List<CommunicateCoordinate> CommunicateCoordinates { get; set; }=new(); 
    public virtual List<CommunicateDevice> CommunicateDevices { get; set; } = new();
    public virtual List<CommunicateModel> CommunicateModels { get; set; } = new();
    public virtual List<CommunicateSpace> CommunicateSpaces { get; set; } = new();
    public virtual List<CommunicateCategory> CommunicateCategories { get; set; } = new();

}
