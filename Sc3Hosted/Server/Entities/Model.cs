
namespace Sc3Hosted.Server.Entities;

public class Model 
{
    public int ModelId { get; set; }
    public string Name { get; set; }

    public Model(string name)
    {
        Name = name;
    }

    public int DeviceId { get; set; }
    public Device? Device { get; set; } 
    public int ModelParameterId { get; set; }
    public List<ModelParameter>? ModelParameters { get; set; } 
    public List<Asset>? Assets { get; set; } 
}