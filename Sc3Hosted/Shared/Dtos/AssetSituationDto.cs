using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sc3Hosted.Shared.Dtos;
public class AssetSituationDto : BaseDto
{
    public int AssetSituationId { get; set; }
    public int AssetId { get; set; }
    public int SituationId { get; set; }
}
