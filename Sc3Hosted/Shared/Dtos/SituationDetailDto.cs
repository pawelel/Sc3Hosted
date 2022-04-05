using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sc3Hosted.Shared.Dtos;
public class SituationDetailDto : BaseDto
{
    public int SituationDetailId { get; set; }
    public int SituationId { get; set; }
    public int DetailId { get; set; }
}
