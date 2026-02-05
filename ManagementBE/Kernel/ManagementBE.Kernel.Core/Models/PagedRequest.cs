using ManagementBE.Kernel.Core.Constants;
using System;
using System.Collections.Generic;
using System.Text;

namespace ManagementBE.Kernel.Core.Models
{
    public class PagedRequest
    {
        public int PageSize { get; set; }
        public int PageNumber { get; set; } = 1;
    }
}
