using ManagementBE.Kernel.Core.Constants;
using ManagementBE.Kernel.Core.Persistence;
using ManagementBE.Kernel.Domain.Logs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementBE.Kernel.Infrastructure.Persistence.Mapping.Logging
{
    public class ApplicationLogMapping : IEntityTypeConfiguration<ApplicationLog>, IEntityMapping
    {
        public void Configure(EntityTypeBuilder<ApplicationLog> builder)
        {
            builder.ToTable("ApplicationLog", SchemaName.Log);
            builder.HasKey(t => t.Id);
            builder.Property(p => p.LoggedAt).HasColumnType("datetime").IsRequired();
            builder.Property(p => p.UserName).HasMaxLength(100);
            builder.Property(p => p.Service).HasMaxLength(500);
            builder.Property(p => p.ResultCode).HasMaxLength(50);
            builder.Property(p => p.ResultType).HasMaxLength(50);
            builder.Property(p => p.IPAddress).HasMaxLength(50);
            builder.Property(p => p.Device).HasMaxLength(200);
            builder.Property(p => p.BrowserName).HasMaxLength(50);
            builder.Property(p => p.UserAgent).HasMaxLength(500);
        }
    }
}
