using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementBE.Kernel.Core.Configurations
{
    public class IdentitySettings
    {
        public int LockoutDurationInMinutes { get; set; }
        public int MaxFailedAccessAttempts { get; set; }
    }
}
