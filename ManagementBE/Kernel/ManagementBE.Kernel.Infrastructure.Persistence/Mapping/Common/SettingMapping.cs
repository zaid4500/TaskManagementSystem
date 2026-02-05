using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ManagementBE.Kernel.Core.Constants;
using ManagementBE.Kernel.Domain.Common;

namespace ManagementBE.Kernel.Infrastructure.Persistence.Mapping.Common
{
    /// <summary>
    /// Setting Mapping
    /// </summary>
    public class SettingMapping : EntityMapping<Setting,string>
    {
        public override void Configure(EntityTypeBuilder<Setting> builder)
        {
            base.Configure(builder);
            builder.ToTable("Settings", SchemaName.Common);
            builder.HasKey(x => x.Id);
            builder.Property(p => p.Id).HasMaxLength(500);

        }
    }
}
