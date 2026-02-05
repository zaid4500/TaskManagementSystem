using System;

namespace ManagementBE.Kernel.Domain.Entities.Base
{
    public class FullAuditableEntity<T> : Entity<T>, IAuditableEntity, ISoftDeletedEntity
    {
        public string CreatedBy { get; set; } = null!;
        public DateTime CreatedOn { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string? DeletedBy { get; set; }
        public DateTime? DeletedOn { get; set; }
        public bool? IsDeleted { get; set; }
    }
}
