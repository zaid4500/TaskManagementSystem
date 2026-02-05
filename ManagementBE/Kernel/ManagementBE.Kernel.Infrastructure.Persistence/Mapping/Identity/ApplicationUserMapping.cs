using ManagementBE.Kernel.Core.Constants;
using ManagementBE.Kernel.Core.Persistence;
using ManagementBE.Kernel.Domain.Identity;
using ManagementBE.Kernel.Domain.Logs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ManagementBE.Kernel.Infrastructure.Persistence.Mapping.Logging
{
    public class ApplicationUserMapping : IEntityTypeConfiguration<ApplicationUser>, IEntityMapping
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.ToTable("Users", SchemaName.Identity);

            builder.HasKey(x => x.Id);
            builder.Property(p => p.Id).HasMaxLength(450);
            builder.Property(p => p.FirstName).HasMaxLength(100).IsRequired(false);
            builder.Property(p => p.MiddleName).HasMaxLength(100).IsRequired(false);
            builder.Property(p => p.LastName).HasMaxLength(100).IsRequired(false);
            builder.Property(p => p.FullNameEn).HasMaxLength(300).IsRequired();
            builder.Property(p => p.FirstNameAr).HasMaxLength(100).IsRequired();
            builder.Property(p => p.MiddleNameAr).HasMaxLength(100).IsRequired();
            builder.Property(p => p.LastNameAr).HasMaxLength(100).IsRequired();
            builder.Property(p => p.FullNameAr).HasMaxLength(300).IsRequired();
            builder.Property(p => p.DateOfBirth).HasColumnType("date").IsRequired();
            builder.Property(p => p.Gender).IsRequired(false);
            builder.Property(p => p.Nationality).HasMaxLength(100).IsRequired(false);
            builder.Property(p => p.MobileNumber).HasMaxLength(20).IsRequired(false);
            builder.Property(p => p.Email).HasMaxLength(256).IsRequired(false);
            builder.Property(p => p.IsActive).IsRequired().HasDefaultValue(true);
            builder.Property(p => p.RefreshToken).HasMaxLength(500).IsRequired(false);
            builder.Property(p => p.RefreshTokenExpiresOn).IsRequired(false);
            builder.Property(p => p.RefreshTokenCreatedByIp).HasMaxLength(50).IsRequired(false);
            builder.Property(p => p.RefreshTokenCreatedOn).IsRequired(false);
            builder.Property(p => p.RefreshTokenRevokedByIp).HasMaxLength(50).IsRequired(false);
            builder.Property(p => p.RefreshTokenRevokedOn).IsRequired(false);
            builder.Property(p => p.CreatedBy).HasMaxLength(450).IsRequired();
            builder.Property(p => p.CreatedOn).IsRequired().HasDefaultValueSql("GETUTCDATE()");
            builder.Property(p => p.ModifiedBy).HasMaxLength(450).IsRequired(false);
            builder.Property(p => p.ModifiedOn).IsRequired(false);
            builder.Property(p => p.DeletedBy).HasMaxLength(450).IsRequired(false);
            builder.Property(p => p.DeletedOn).IsRequired(false);
            builder.Property(p => p.IsDeleted).IsRequired(false).HasDefaultValue(false);
            
            builder.HasIndex(x => x.Email).HasDatabaseName("IX_Users_Email");
            builder.HasIndex(x => x.RefreshToken).HasDatabaseName("IX_Users_RefreshToken");
            builder.HasIndex(x => x.IsActive).HasDatabaseName("IX_Users_IsActive");
            builder.HasIndex(x => x.IsDeleted).HasDatabaseName("IX_Users_IsDeleted");
            builder.HasIndex(x => new { x.IsActive, x.IsDeleted }).HasDatabaseName("IX_Users_IsActive_IsDeleted");
            builder.HasIndex(x => x.CreatedOn).HasDatabaseName("IX_Users_CreatedOn");
            builder.HasQueryFilter(x => x.IsDeleted == false || x.IsDeleted == null);

            builder.Ignore(x => x.FullName);
            builder.Ignore(x => x.FullNameArabic);
            builder.Ignore(x => x.IsRefreshTokenActive);
        }
    }
}
