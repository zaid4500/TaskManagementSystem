using ManagementBE.Kernel.Core.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace ManagementBE.Kernel.Infrastructure.Persistence.Mapping.Identity
{
    public class IdentityUserTokenMapping : IEntityTypeConfiguration<IdentityUserToken<string>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserToken<string>> builder)
        {
            builder.ToTable("UserTokens", SchemaName.Identity);
            builder.HasKey(x => new { x.UserId, x.LoginProvider, x.Name });
            builder.Property(x => x.LoginProvider).HasMaxLength(128);
            builder.Property(x => x.Name).HasMaxLength(128);
        }
    }
}
