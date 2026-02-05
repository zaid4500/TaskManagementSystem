using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementBE.Kernel.Domain.DTOs.Common
{
    public class LookupCategoryDto
    {
        public int Id { get; set; }
        public string NameEn { get; set; } = string.Empty;
        public string NameAr { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public List<LookupDto> Lookups { get; set; } = new List<LookupDto>();
    }
}
