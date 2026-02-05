using ManagementBE.Kernel.Domain.Common;
using ManagementBE.Kernel.Domain.Entities.Base;
using ManagementBE.Kernel.Domain.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementBE.Kernel.Domain.Tasks
{
    public class TaskEntity : Entity<Guid>, IAuditableEntity, ISoftDeletedEntity
    {
        public string Title { get; set; }
        public string Descreption { get; set; }
        public string AssignedToUserId { get; set; }
        public int StatusId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string? DeletedBy { get; set; }
        public DateTime? DeletedOn { get; set; }
        public bool? IsDeleted { get; set; }

        public virtual ApplicationUser AssignedToUser { get; set; } = null!;
        public virtual Lookup Status { get; set; } = null!;
    }
}
