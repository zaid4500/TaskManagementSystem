using ManagementBE.Kernel.Core.Constants;
using ManagementBE.Kernel.Core.Persistence;
using ManagementBE.Kernel.Domain.Logs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementBE.Kernel.Infrastructure.Persistence.Mapping.Logging
{
    public class IdentityRoleMapping : IEntityTypeConfiguration<IdentityRole>, IEntityMapping
    {
        public void Configure(EntityTypeBuilder<IdentityRole> builder)
        {
            builder.ToTable("Roles", SchemaName.Identity);
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasMaxLength(450);
            builder.Property(x => x.Name).HasMaxLength(256);
            builder.Property(x => x.NormalizedName).HasMaxLength(256);
            builder.HasIndex(x => x.NormalizedName).HasDatabaseName("IX_Roles_NormalizedName").IsUnique().HasFilter("[NormalizedName] IS NOT NULL");
        }
    }
}
