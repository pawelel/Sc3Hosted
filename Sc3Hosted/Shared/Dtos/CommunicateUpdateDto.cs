using Sc3Hosted.Shared.Enumerations;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sc3Hosted.Shared.Dtos;
public class CommunicateUpdateDto
{
    public string Name { get; set; }

    public CommunicateUpdateDto(string name)
    {
        Name = name;
    }

    public string? Description { get; set; }
    public Scope Scope { get; set; }
    public CommunicationType CommunicationType { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsArchived { get; set; } = false;
    public List<AreaCommunicateDto> AreaCommunicates { get; set; }
    public List<AssetCommunicateDto> AssetCommunicates { get; set; }
    public List<CoordinateCommunicateDto> CoordinateCommunicates { get; set; }
    public List<DeviceCommunicateDto> DeviceCommunicates { get; set; }
    public List<ModelCommunicateDto> ModelCommunicates { get; set; }
    public List<SpaceCommunicateDto> SpaceCommunicates { get; set; }
}
