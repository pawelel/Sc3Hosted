
namespace Sc3Hosted.Server.Entities;
public class Situation  
{
    public int SituationId { get; set; }
    public string Name { get; set; }

    public Situation(string name)
    {
        Name = name;
    }

    public List<SituationQuestion>? SituationQuestions { get; set; } 
    public List<SituationDetail>? SituationDetails { get; set; }
    public List<SituationParameter>? SituationParameters { get; set; }
    public List<SituationCategory>? SituationCategories { get; set; } 
    public List<SituationAsset>? SituationAssets { get; set; } 

}
