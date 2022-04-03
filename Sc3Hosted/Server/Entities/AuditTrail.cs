using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sc3Hosted.Server.Entities;
public class AuditTrail
{
    public long AuditTrailId { get; set; }
    public string? Table { get; set; }
    public string? Column { get; set; }
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public string? UserId { get; set; }
    public DateTime ChangeDate { get; set; }
}
