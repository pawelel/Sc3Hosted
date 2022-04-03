namespace Sc3Hosted.Server.Entities;

public class SituationDetail 
{
    public int SituationDetailId { get; set; }
    public int SituationId { get; set; }
    public int DetailId { get; set; }
    public Situation? Situation { get; set; }
    public Detail? Detail { get; set; }
}