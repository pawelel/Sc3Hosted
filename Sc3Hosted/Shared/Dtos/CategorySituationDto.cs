using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sc3Hosted.Shared.Dtos;
public class CategorySituationDto : BaseDto
{
    public int CategorySituationId { get; set; }
    public int CategoryId { get; set; }
    public int SituationId { get; set; }
}
