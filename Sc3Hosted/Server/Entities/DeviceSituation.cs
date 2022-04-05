namespace Sc3Hosted.Server.Entities;

public class DeviceSituation :BaseEntity
{
    public int DeviceSituationId { get; set; }
    public virtual Situation Situation { get; set; } = new();
    public int SituationId { get; set; }
    public virtual Device Device { get; set; } = new();
    public int DeviceId { get; set; }
}
