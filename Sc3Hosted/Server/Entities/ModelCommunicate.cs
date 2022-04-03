namespace Sc3Hosted.Server.Entities;
public class ModelCommunicate
{
    public int ModelCommunicateId { get; set; }
    public int ModelId { get; set; }
    public Model? Model { get; set; } 
    public int CommunicateId { get; set; }
    public Communicate? Communicate { get; set; } 
}
