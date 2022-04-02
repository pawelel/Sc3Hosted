using Sc3Hosted.Shared.Enumerations;

namespace Sc3Hosted.Shared.Dtos;
public class AssetCreateDto
{
    public string Name { get; set; }

    public AssetCreateDto(string name)
    {
        Name = name;
    }
    public int ModelId { get; set; }
    public Status Status { get; set; }
    public int CoordinateId { get; set; }
    public string? Process { get; set; }
}
