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
    public bool IsArchived { get; set; }
    public virtual List<CommunicateArea> AreaCommunicates { get; set; } = new();
    public virtual List<CommunicateAsset> AssetCommunicates { get; set; }  = new();
    public virtual List<CommunicateCoordinate> CoordinateCommunicates { get; set; }=new(); 
    public virtual List<CommunicateDevice> DeviceCommunicates { get; set; } = new();
    public virtual List<CommunicateModel> ModelCommunicates { get; set; } = new();
    public virtual List<CommunicateSpace> SpaceCommunicates { get; set; } = new();

}
