using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sc3Hosted.Shared.Dtos;
public class QuestionWithSituationsDto
{
    public int QuestionId { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<SituationDto> Situations { get; set; } = new();
}
