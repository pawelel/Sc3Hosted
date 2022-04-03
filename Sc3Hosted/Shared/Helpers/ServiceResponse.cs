using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Sc3Hosted.Shared.Helpers
{
    public class ServiceResponse<T>
    {
        public bool Success { get; set; } = true;
        public T Data { get; set; } = default(T)!;
        public string Message { get; set; } = string.Empty;
    }

}