using Sc3Hosted.Shared.Enumerations;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sc3Hosted.Shared.Dtos;
public class CommunicateUpdateDto
{
    public string Name { get; set; }= string.Empty;
    
    public string Description { get; set; } = string.Empty;
    public Scope Scope { get; set; }
    public CommunicationType CommunicationType { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    
    public List<AreaCommunicateDto> AreaCommunicates { get; set; } = new();
    public List<AssetCommunicateDto> AssetCommunicates { get; set; } = new();
    public List<CoordinateCommunicateDto> CoordinateCommunicates { get; set; } = new();
    public List<DeviceCommunicateDto> DeviceCommunicates { get; set; } = new();
    public List<ModelCommunicateDto> ModelCommunicates { get; set; } = new();
    public List<SpaceCommunicateDto> SpaceCommunicates { get; set; } = new();
}
