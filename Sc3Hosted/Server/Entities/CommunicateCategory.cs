namespace Sc3Hosted.Server.Entities;

public class CommunicateCategory : BaseEntity
{
 
    public int CommunicateId { get; set; }
    public virtual Communicate Communicate { get; set; } = new();
    public int CategoryId { get; set; }
    public virtual Category Category { get; set; } = new();

}