using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementBE.Kernel.Domain.DTOs.Identity
{
    public class RegisterDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string? FullNameEn { get; set; } = string.Empty;
        public string FirstNameAr { get; set; } = string.Empty;
        public string? MiddleNameAr { get; set; } = string.Empty;
        public string LastNameAr { get; set; } = string.Empty;
        public string? FullNameAr { get; set; } = string.Empty;
        public DateTime? DateOfBirth { get; set; }
        public int? GenderId { get; set; }
        public string? Nationality { get; set; }
        public string? PhoneNumber { get; set; }
        public string? MobileNumber { get; set; }
        public string Role { get; set; }
    }
}
