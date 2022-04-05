

namespace Sc3Hosted.Shared.Dtos;
public class ParameterDto : BaseDto
{
    public int ParameterId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
