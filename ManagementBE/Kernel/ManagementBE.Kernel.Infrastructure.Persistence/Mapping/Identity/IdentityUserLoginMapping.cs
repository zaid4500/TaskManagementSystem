using ManagementBE.Kernel.Core.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace ManagementBE.Kernel.Infrastructure.Persistence.Mapping.Identity
{
    public class IdentityUserLoginMapping : IEntityTypeConfiguration<IdentityUserLogin<string>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserLogin<string>> builder)
        {
            builder.ToTable("UserLogins", SchemaName.Identity);
            builder.HasKey(x => new { x.LoginProvider, x.ProviderKey });
            builder.Property(x => x.LoginProvider).HasMaxLength(128);
            builder.Property(x => x.ProviderKey).HasMaxLength(128);
            builder.HasIndex(x => x.UserId).HasDatabaseName("IX_UserLogins_UserId");
        }
    }
}
