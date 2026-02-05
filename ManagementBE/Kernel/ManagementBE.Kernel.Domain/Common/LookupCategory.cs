using ManagementBE.Kernel.Domain.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementBE.Kernel.Domain.Common
{
    public class LookupCategory : Entity<int>
    {
        public string NameEn { get; set; } = string.Empty;
        public string NameAr { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;

        public virtual ICollection<Lookup> Lookups { get; set; } = new List<Lookup>();
    }
}
