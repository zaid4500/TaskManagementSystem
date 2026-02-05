using ManagementBE.Kernel.Core.Constants;
using ManagementBE.Kernel.Core.Persistence;
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
    public class IdentityUserRoleMapping : IEntityTypeConfiguration<IdentityUserRole<string>>, IEntityMapping
    {
        public void Configure(EntityTypeBuilder<IdentityUserRole<string>> builder)
        {
            builder.ToTable("UserRoles", SchemaName.Identity);
            builder.HasKey(x => new { x.UserId, x.RoleId });
            builder.HasIndex(x => x.RoleId).HasDatabaseName("IX_UserRoles_RoleId");
        }
    }
}
