using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sc3Hosted.Server.Entities;
public class SituationParameter : BaseEntity
{
    public int SituationId { get; set; }
    public int ParameterId { get; set; }
    public virtual Situation Situation { get; set; } = new();
    public virtual Parameter Parameter { get; set; } = new();
}
