using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementBE.Kernel.Domain.DTOs.Common
{
    public class SettingDto
    {
        public string Id { get; set; }
        public string Value { get; set; } = null!;
    }
}
