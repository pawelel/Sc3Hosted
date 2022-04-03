using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sc3Hosted.Server.Entities
{
    public class BaseEntity
    {
        public string UserId { get; set; }="App";
        public bool IsDeleted { get; set; }
    }
}