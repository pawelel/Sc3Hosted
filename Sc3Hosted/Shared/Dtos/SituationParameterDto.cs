using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sc3Hosted.Shared.Dtos;
public class SituationParameterDto : BaseDto
{
    public int SituationId { get; set; }
    public int ParameterId { get; set; }
}
