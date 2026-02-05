using System;
using System.Collections.Generic;
using System.Text;

namespace ManagementBE.Kernel.Domain.Entities.Base
{
    public interface IAuditableEntity
    {
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }
}
