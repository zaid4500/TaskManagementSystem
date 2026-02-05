using ManagementBE.Kernel.Core.Constants;
using ManagementBE.Kernel.Domain.Common;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementBE.Kernel.Infrastructure.Persistence.Mapping.Common
{
    public class LookupCategoryMapping : EntityMapping<LookupCategory, int>
    {
        public void Configure(EntityTypeBuilder<LookupCategory> builder)
        {
            builder.ToTable("LookupCategories", SchemaName.Common);

            builder.HasKey(x => x.Id);

            builder.Property(p => p.NameEn).HasMaxLength(200).IsRequired();
            builder.Property(p => p.NameAr).HasMaxLength(200).IsRequired();
            builder.Property(p => p.Code).HasMaxLength(50).IsRequired();

            builder.HasMany(c => c.Lookups)
                .WithOne(l => l.LookupCategory)
                .HasForeignKey(l => l.LookupCategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.Code).HasDatabaseName("IX_LookupCategories_Code").IsUnique();
        }
    }
}
