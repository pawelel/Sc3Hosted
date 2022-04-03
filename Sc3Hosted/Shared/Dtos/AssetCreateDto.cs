using Sc3Hosted.Shared.Enumerations;

namespace Sc3Hosted.Shared.Dtos;
public class AssetCreateDto
{
    public string Name { get; set; }= string.Empty;

    
    public int ModelId { get; set; }
    public Status Status { get; set; }
    public int CoordinateId { get; set; }
    public string Process { get; set; }= string.Empty;
}
