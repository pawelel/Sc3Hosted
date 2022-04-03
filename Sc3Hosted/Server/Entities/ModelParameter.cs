namespace Sc3Hosted.Server.Entities;

public class ModelParameter 
{
    public int ModelParameterId { get; set; }
    public int ModelId { get; set; }
    public Model? Model { get; set; } 
    public Parameter? Parameter { get; set; } 
    public int ParameterId { get; set; }
    public string? Value { get; set; }
}