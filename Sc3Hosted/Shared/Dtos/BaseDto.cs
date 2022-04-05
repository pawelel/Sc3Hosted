using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sc3Hosted.Shared.Dtos;
public class BaseDto
{
    public string UserId { get; set; } = string.Empty;
    public bool IsDeleted { get; set; }
}
