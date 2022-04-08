using Sc3Hosted.Shared.Enumerations;

namespace Sc3Hosted.Shared.Dtos;
public class CommunicateDto : BaseDto
{
    public int CommunicateId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Scope Scope { get; set; }
    public CommunicationType CommunicationType { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    
    public virtual List<CommunicateAreaDto> CommunicateAreas { get; set; } = new();
    public virtual List<CommunicateAssetDto> CommunicateAssets { get; set; } = new();
    public virtual List<CommunicateCoordinateDto> CommunicateCoordinates { get; set; } = new();
    public virtual List<CommunicateDeviceDto> CommunicateDevices { get; set; } = new();
    public virtual List<CommunicateModelDto> CommunicateModels { get; set; } = new();
    public virtual List<CommunicateSpaceDto> CommunicateSpaces { get; set; } = new();
    public virtual List<CommunicateCategoryDto> CommunicateCategories { get; set; } = new();
}
