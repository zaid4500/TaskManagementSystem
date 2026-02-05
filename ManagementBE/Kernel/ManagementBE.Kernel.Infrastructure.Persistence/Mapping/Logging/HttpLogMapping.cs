using ManagementBE.Kernel.Core.Constants;
using ManagementBE.Kernel.Core.Persistence;
using ManagementBE.Kernel.Domain.Logs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ManagementBE.Kernel.Infrastructure.Persistence.Mapping.Logging
{
    public class HttpLogMapping : IEntityTypeConfiguration<HttpLog>, IEntityMapping
    {
        public void Configure(EntityTypeBuilder<HttpLog> builder)
        {
            builder.ToTable("HttpLogs", SchemaName.Log);
            builder.HasKey(t => t.Id);
            builder.Property(p => p.RequestDateTime).HasColumnType("datetime")
                .IsRequired()
                .HasDefaultValue(DateTime.Now);

            builder.Property(p => p.IPAddress).HasMaxLength(50);
            builder.Property(p => p.Node).HasMaxLength(50);
            builder.Property(p => p.RequestMethod).HasColumnType("nvarchar(MAX)");
            builder.Property(p => p.RequestPath).HasMaxLength(500);
            builder.Property(p => p.RequestQuery).HasColumnType("nvarchar(MAX)");
            builder.Property(p => p.RequestQueries).HasColumnType("nvarchar(MAX)");
            builder.Property(p => p.RequestScheme).HasColumnType("nvarchar(MAX)");
            builder.Property(p => p.RequestHost).HasColumnType("nvarchar(MAX)");
            builder.Property(p => p.RequestHeaders).HasColumnType("nvarchar(MAX)");
            builder.Property(p => p.RequestBody).HasColumnType("nvarchar(MAX)");
            builder.Property(p => p.RequestContentType).HasColumnType("nvarchar(MAX)");
            builder.Property(p => p.RequestContentType).HasColumnType("nvarchar(MAX)");
            builder.Property(p => p.ResponseStatus).HasColumnType("nvarchar(MAX)");
            builder.Property(p => p.ResponseHeaders).HasColumnType("nvarchar(MAX)");
            builder.Property(p => p.ResponseBody).HasColumnType("nvarchar(MAX)");
            builder.Property(p => p.ResponseContentType).HasColumnType("nvarchar(MAX)");
            builder.Property(p => p.ResponseDateTime).HasColumnType("datetime");
            builder.Property(p => p.ActionPerionTime).HasColumnType("float");
            builder.Property(p => p.ExceptionMessage).HasColumnType("nvarchar(MAX)");
            builder.Property(p => p.ExceptionStackTrace).HasColumnType("nvarchar(MAX)");
            builder.Property(p => p.ExceptionDetails).HasColumnType("nvarchar(MAX)");
            builder.Property(p => p.Username).HasMaxLength(200);
        }
    }
}
