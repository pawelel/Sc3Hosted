

namespace Sc3Hosted.Shared.Dtos;

public class ModelDto
{
    public int ModelId { get; set; }
    public string? Name { get; set; }

    public List<ModelParameterDto> ModelParameters { get; set; } 
    public List<AssetDto> Assets { get; set; }
}