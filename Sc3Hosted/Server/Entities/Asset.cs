using Sc3Hosted.Shared.Enumerations;

namespace Sc3Hosted.Server.Entities;

public class Asset 
{
    public int AssetId { get; set; }
    public string Name { get; set; }

    public Asset(string name)
    {
        Name = name;
    }
    public int ModelId { get; set; }
    public Model? Model { get; set; }
    public Status Status { get; set; }
    public Coordinate? Coordinate { get; set; }
    public int CoordinateId { get; set; }
    public virtual List<AssetCategory>? AssetCategories { get; set; } 
    public virtual List<AssetDetail>? AssetDetails { get; set; }
    public virtual List<SituationAsset>? AssetSituations { get; set; }
    public string? Process { get; set; }
    public virtual List<AssetCommunicate>? AssetCommunicates { get; set; }
}