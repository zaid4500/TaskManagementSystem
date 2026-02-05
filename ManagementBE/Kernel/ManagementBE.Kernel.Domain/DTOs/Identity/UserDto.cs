using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementBE.Kernel.Domain.DTOs.Identity
{
    public class UserDto
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string FullNameEn { get; set; } = string.Empty;
        public string FirstNameAr { get; set; } = string.Empty;
        public string MiddleNameAr { get; set; } = string.Empty;
        public string LastNameAr { get; set; } = string.Empty;
        public string FullNameAr { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public int? GenderId { get; set; }
        public string? GenderDisplay { get; set; }
        public string? Nationality { get; set; }
        public string? MobileNumber { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string Role { get; set; }
    }
}
