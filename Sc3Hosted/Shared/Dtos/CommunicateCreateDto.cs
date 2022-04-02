
using Sc3Hosted.Shared.Enumerations;

namespace Sc3Hosted.Shared.Dtos;
public class CommunicateCreateDto
{
  
    public string Name { get; set; }

    public CommunicateCreateDto(string name)
    {
        Name = name;
    }

    public string? Description { get; set; }
    public Scope Scope { get; set; }
    public CommunicationType CommunicationType { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsArchived { get; set; } = false;
}
