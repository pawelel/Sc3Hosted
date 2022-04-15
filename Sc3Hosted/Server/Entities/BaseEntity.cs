namespace Sc3Hosted.Server.Entities;
public abstract class BaseEntity
{
    public string CreatedBy { get; set; } = "App";
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public string UpdatedBy { get; set; } = "App";
}
