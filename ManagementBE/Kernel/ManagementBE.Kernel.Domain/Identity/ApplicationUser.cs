using ManagementBE.Kernel.Domain.Entities.Base;
using Microsoft.AspNetCore.Identity;
using System.Runtime.CompilerServices;

namespace ManagementBE.Kernel.Domain.Identity
{
    public class ApplicationUser : IdentityUser, IAuditableEntity, ISoftDeletedEntity
    {
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string FullNameEn { get; set; }
        public string FirstNameAr { get; set; }
        public string MiddleNameAr { get; set; }
        public string LastNameAr { get; set; }
        public string FullNameAr { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int? Gender { get; set; } 
        public string? Nationality { get; set; }
        public string? MobileNumber { get; set; }
        public string? Email { get; set; }
        public bool IsActive { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiresOn { get; set; }
        public string? RefreshTokenCreatedByIp { get; set; }
        public DateTime? RefreshTokenCreatedOn { get; set; }
        public string? RefreshTokenRevokedByIp { get; set; }
        public DateTime? RefreshTokenRevokedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string? DeletedBy { get; set; }
        public DateTime? DeletedOn { get; set; }
        public bool? IsDeleted { get; set; }

        public string FullName
        {
            get
            {
                if (!string.IsNullOrEmpty(FirstName) && !string.IsNullOrEmpty(LastName))
                {
                    return string.IsNullOrEmpty(MiddleName)
                        ? $"{FirstName} {LastName}"
                        : $"{FirstName} {MiddleName} {LastName}";
                }
                return FullNameEn;
            }
        }

        public string FullNameArabic
        {
            get
            {
                if (!string.IsNullOrEmpty(FirstNameAr) && !string.IsNullOrEmpty(LastNameAr))
                {
                    return string.IsNullOrEmpty(MiddleNameAr)
                        ? $"{FirstNameAr} {LastNameAr}"
                        : $"{FirstNameAr} {MiddleNameAr} {LastNameAr}";
                }
                return FullNameAr;
            }
        }

        public bool IsRefreshTokenActive
        {
            get
            {
                return RefreshToken != null &&
                       RefreshTokenExpiresOn.HasValue &&
                       RefreshTokenExpiresOn.Value > DateTime.UtcNow &&
                       RefreshTokenRevokedOn == null;
            }
        }
    }
}
