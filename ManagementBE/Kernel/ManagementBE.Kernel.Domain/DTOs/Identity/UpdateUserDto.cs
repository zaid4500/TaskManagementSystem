using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementBE.Kernel.Domain.DTOs.Identity
{
    public class UpdateUserDto
    {
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string? FullNameEn { get; set; }
        public string? FirstNameAr { get; set; }
        public string? MiddleNameAr { get; set; }
        public string? LastNameAr { get; set; }
        public string? FullNameAr { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public int? Gender { get; set; }
        public string? Nationality { get; set; }
        public string? PhoneNumber { get; set; }
        public string? MobileNumber { get; set; }
    }
}
