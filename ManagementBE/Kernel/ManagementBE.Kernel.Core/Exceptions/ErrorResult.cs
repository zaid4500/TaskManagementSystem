using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementBE.Kernel.Core.Exceptions
{
    public class ErrorResult
    {
        //public List<string> Messages { get; set; } = new();

        public string? Source { get; set; }
        public string? Exception { get; set; }
        public string? ErrorId { get; set; }
        public string? Message { get; set; }
        public int StatusCode { get; set; }
    }
}
