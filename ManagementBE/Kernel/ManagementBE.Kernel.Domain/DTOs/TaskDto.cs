using ManagementBE.Kernel.Domain.DTOs.Common;
using ManagementBE.Kernel.Domain.DTOs.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementBE.Kernel.Domain.DTOs
{
    public class TaskDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Descreption { get; set; }
        public string AssignedToUserId { get; set; }
        public int StatusId { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string? DeletedBy { get; set; }
        public DateTime? DeletedOn { get; set; }
        public bool? IsDeleted { get; set; }

        public UserDto? AssignedToUser { get; set; }
        public LookupDto? Status { get; set; }


    }
}
