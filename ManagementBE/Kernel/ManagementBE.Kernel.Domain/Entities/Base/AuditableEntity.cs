using System;
using System.Collections.Generic;
using System.Text;

namespace ManagementBE.Kernel.Domain.Entities.Base
{
    public class AuditableEntity<T> : Entity<T>, IAuditableEntity
    {
        public string CreatedBy { get; set; } = null!;
        public DateTime CreatedOn { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }
}
