using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sc3Hosted.Shared.Dtos;
public class CategoryUpdateDto
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public CategoryUpdateDto(string name)
    {
        Name = name;
    }
}
