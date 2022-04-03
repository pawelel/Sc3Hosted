using Sc3Hosted.Shared.Enumerations;

namespace Sc3Hosted.Server.Entities;
public class Communicate 
{
    public int CommunicateId { get; set; }
    public string Name { get; set; }

    public Communicate(string name)
    {
        Name = name;
    }

    public string? Description { get; set; } 
    public Scope Scope { get; set; }
    public CommunicationType CommunicationType { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsArchived { get; set; }
    public List<AreaCommunicate>? AreaCommunicates { get; set; } 
    public List<AssetCommunicate>? AssetCommunicates { get; set; } 
    public List<CoordinateCommunicate>? CoordinateCommunicates { get; set; } 
    public List<DeviceCommunicate>? DeviceCommunicates { get; set; } 
    public List<ModelCommunicate>? ModelCommunicates { get; set; } 
    public List<SpaceCommunicate>? SpaceCommunicates { get; set; } 

}
