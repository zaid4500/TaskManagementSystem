using ManagementBE.Kernel.Core.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace ManagementBE.Kernel.Infrastructure.Persistence.Mapping.Identity
{
    public class IdentityRoleClaimMapping : IEntityTypeConfiguration<IdentityRoleClaim<string>>
    {
        public void Configure(EntityTypeBuilder<IdentityRoleClaim<string>> builder)
        {
            builder.ToTable("RoleClaims", SchemaName.Identity);
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => x.RoleId).HasDatabaseName("IX_RoleClaims_RoleId");
        }
    }
}
