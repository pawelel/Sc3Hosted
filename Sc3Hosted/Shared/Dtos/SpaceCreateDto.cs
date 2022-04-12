using Sc3Hosted.Shared.Enumerations;
namespace Sc3Hosted.Shared.Dtos;
public class SpaceCreateDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public SpaceType SpaceType { get; set; }
}
