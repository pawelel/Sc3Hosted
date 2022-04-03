namespace Sc3Hosted.Server.Entities;

public class SituationCategory 
{
    public int SituationCategoryId { get; set; }
    public int SituationId { get; set; }
    public int CategoryId { get; set; }
    public Situation? Situation { get; set; } 
    public Category ?Category { get; set; } 
}