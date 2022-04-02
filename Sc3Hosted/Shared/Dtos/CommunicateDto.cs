using Sc3Hosted.Shared.Enumerations;

namespace Sc3Hosted.Shared.Dtos;
public class CommunicateDto
{
    public int CommunicateId { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public Scope Scope { get; set; }
    public CommunicationType CommunicationType { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsArchived { get; set; } = false;
    public List<AreaCommunicateDto>? AreaCommunicates { get; set; } 
    public List<AssetCommunicateDto>? AssetCommunicates { get; set; } 
    public List<CoordinateCommunicateDto>? CoordinateCommunicates { get; set; } 
    public List<DeviceCommunicateDto>? DeviceCommunicates { get; set; } 
    public List<ModelCommunicateDto>? ModelCommunicates { get; set; } 
    public List<SpaceCommunicateDto>? SpaceCommunicates { get; set; }
}
