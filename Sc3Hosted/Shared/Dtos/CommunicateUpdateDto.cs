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
    
    public virtual List<CommunicateAreaDto> CommunicateAreas { get; set; } = new();
    public virtual List<CommunicateAssetDto> CommunicateAssets { get; set; } = new();
    public virtual List<CommunicateCoordinateDto> CommunicateCoordinates { get; set; } = new();
    public virtual List<CommunicateDeviceDto> CommunicateDevices { get; set; } = new();
    public virtual List<CommunicateModelDto> CommunicateModels { get; set; } = new();
    public virtual List<CommunicateSpaceDto> CommunicateSpaces { get; set; } = new();
}
