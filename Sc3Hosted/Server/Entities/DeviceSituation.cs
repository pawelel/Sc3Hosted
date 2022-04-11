namespace Sc3Hosted.Server.Entities;

public class DeviceSituation :BaseEntity
{
    public virtual Situation Situation { get; set; } = new();
    public int SituationId { get; set; }
    public virtual Device Device { get; set; } = new();
    public int DeviceId { get; set; }
}
