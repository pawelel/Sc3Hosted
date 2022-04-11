

namespace Sc3Hosted.Shared.Dtos;

public class ModelParameterDto : BaseDto
{ 
    public string Value { get; set; }=  string.Empty;
    public int ModelId { get; set; }
    public int ParameterId { get; set; }
}