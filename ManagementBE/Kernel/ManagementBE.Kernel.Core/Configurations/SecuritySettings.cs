using Microsoft.Diagnostics.Tracing.Parsers.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementBE.Kernel.Core.Configurations
{
    public class SecuritySettings
    {
        public string? Provider { get; set; }
        public bool RequireConfirmedAccount { get; set; }
        public virtual JwtSettings JwtSettings { get; set; } = new();
    }

}
