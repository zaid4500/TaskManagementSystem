using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementBE.Kernel.Core.Configurations
{
    public class RequestResponseLogger
    {
        public bool Enabled { get; set; }
        public string Name { get; set; }
        public string DateTimeFormat { get; set; }
    }
}
