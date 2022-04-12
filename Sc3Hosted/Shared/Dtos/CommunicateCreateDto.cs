using Sc3Hosted.Shared.Enumerations;
namespace Sc3Hosted.Shared.Dtos;
public class CommunicateCreateDto : BaseDto
{

    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Scope Scope { get; set; }
    public CommunicationType CommunicationType { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
