
namespace Sc3Hosted.Server.Entities;
public class Parameter 
{
    public int ParameterId { get; set; }
    public string Name { get; set; }

    public Parameter(string name)
    {
        Name = name;
    }

    public string? Description { get; set; }
    public List<ModelParameter>? ModelParameters { get; set; }
    public List<SituationParameter>? SituationParameters { get; set; }
}
