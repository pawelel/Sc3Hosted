using Sc3Hosted.Shared.Enumerations;
namespace Sc3Hosted.Shared.Dtos;
public class AssetUpdateDto
{
    
    public string Process { get; set; } = string.Empty;
    public Status Status { get; set; }
    public int CoordinateId { get; set; }
    public string Description { get; set; } = string.Empty;
}
