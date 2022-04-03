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
    public virtual List<AreaCommunicate> AreaCommunicates { get; set; } = new();
    public virtual List<AssetCommunicate> AssetCommunicates { get; set; }  = new();
    public virtual List<CoordinateCommunicate> CoordinateCommunicates { get; set; }=new(); 
    public virtual List<DeviceCommunicate> DeviceCommunicates { get; set; } = new();
    public virtual List<ModelCommunicate> ModelCommunicates { get; set; } = new();
    public virtual List<SpaceCommunicate> SpaceCommunicates { get; set; } = new();

}
