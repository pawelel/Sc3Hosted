

namespace Sc3Hosted.Shared.Dtos;

public class AssetDetailDto : BaseDto
{
    public int AssetDetailId { get; set; }
    public int AssetId { get; set; }
    public int DetailId { get; set; }
    public string Value { get; set; }= string.Empty;
}