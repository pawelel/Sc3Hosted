namespace Sc3Hosted.Server.Entities;
public class Detail 
{
    public int DetailId { get; set; }
    public string Name { get; set; }

    public Detail(string name)
    {
        Name = name;
    }

    public string? Description { get; set; }
    public List<SituationDetail>? SituationDetails { get; set; }
    public List<AssetDetail>? AssetDetails { get; set; }
}
