using ManagementBE.Kernel.Core.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementBE.Kernel.Infrastructure.Persistence.Mapping.Identity
{
    public class IdentityUserClaimMapping : IEntityTypeConfiguration<IdentityUserClaim<string>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserClaim<string>> builder)
        {
            builder.ToTable("UserClaims", SchemaName.Identity);
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => x.UserId).HasDatabaseName("IX_UserClaims_UserId");
        }
    }
}
