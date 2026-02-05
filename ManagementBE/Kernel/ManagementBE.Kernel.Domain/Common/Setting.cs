using ManagementBE.Kernel.Domain.Entities.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace ManagementBE.Kernel.Domain.Common
{
    public class Setting : Entity<string>
    {
        public string Value { get; set; } = null!;
    }
}
