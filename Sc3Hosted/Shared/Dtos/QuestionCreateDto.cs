using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sc3Hosted.Shared.Dtos;
public class QuestionCreateDto
{
    public string Name { get; set; }

    public QuestionCreateDto(string name)
    {
        Name = name;
    }
}
