using System;
using System.Collections.Generic;
using System.Text;

namespace ManagementBE.Kernel.Domain.Entities.Base
{
    public interface ISoftDeletedEntity
    {
        public string? DeletedBy { get; set; }
        public DateTime? DeletedOn { get; set; }
        public bool? IsDeleted { get; set; }
    }
}
