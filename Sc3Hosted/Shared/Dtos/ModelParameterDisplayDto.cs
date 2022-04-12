namespace Sc3Hosted.Shared.Dtos;
public class ModelParameterDisplayDto : BaseDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public int ModelId { get; set; }
    public int ParameterId { get; set; }
}
