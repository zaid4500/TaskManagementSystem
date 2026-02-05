using ManagementBE.Kernel.Domain.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementBE.Kernel.Domain.Common
{
    public class Lookup : Entity<int>
    {
        public int LookupCategoryId { get; set; }
        public string NameEn { get; set; } = string.Empty;
        public string NameAr { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;

        // Navigation Properties
        public virtual LookupCategory LookupCategory { get; set; } = null!;
    }
}
