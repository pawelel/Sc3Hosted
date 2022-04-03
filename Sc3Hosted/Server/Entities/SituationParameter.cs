using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sc3Hosted.Server.Entities;
public class SituationParameter 
{
    public int SituationParameterId { get; set; }
    public int SituationId { get; set; }
    public int ParameterId { get; set; }
    public Situation? Situation { get; set; }
    public Parameter? Parameter { get; set; }
}
