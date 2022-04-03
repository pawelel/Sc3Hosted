namespace Sc3Hosted.Server.Entities;

public class Category 
{
    public int CategoryId { get; set; }
    public string Name { get; set; }
public string? Description { get; set; }


    public Category(string name)
    {
        Name = name;
    }

    public List<AssetCategory>? AssetCategories { get; set; } 
    public List<SituationCategory>? CategorySituations { get; set; } 
}
