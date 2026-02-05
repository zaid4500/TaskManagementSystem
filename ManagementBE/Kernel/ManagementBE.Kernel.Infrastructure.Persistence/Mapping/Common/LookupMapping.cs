using ManagementBE.Kernel.Core.Constants;
using ManagementBE.Kernel.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementBE.Kernel.Infrastructure.Persistence.Mapping.Common
{
    public class LookupMapping : EntityMapping<Lookup, int>
    {
        public void Configure(EntityTypeBuilder<Lookup> builder)
        {
            builder.ToTable("Lookups", SchemaName.Common);
            builder.HasKey(x => x.Id);
            builder.Property(p => p.LookupCategoryId).IsRequired();
            builder.Property(p => p.NameEn).HasMaxLength(200).IsRequired();
            builder.Property(p => p.NameAr).HasMaxLength(200).IsRequired();
            builder.Property(p => p.Code).HasMaxLength(50).IsRequired();

            builder.HasIndex(x => new { x.LookupCategoryId, x.Code })
                .HasDatabaseName("IX_Lookups_LookupCategoryId_Code")
                .IsUnique();
        }
    }
}
