using ManagementBE.Kernel.Core.Constants;
using ManagementBE.Kernel.Core.Persistence;
using ManagementBE.Kernel.Domain.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagementBE.Kernel.Domain.Tasks;
using ManagementBE.Kernel.Domain.Common;

namespace ManagementBE.Kernel.Infrastructure.Persistence.Mapping.Tasks
{
    public class TaskEntityMapping : EntityMapping<TaskEntity, Guid>
    {
        public override void Configure(EntityTypeBuilder<TaskEntity> builder)
        {
            builder.ToTable("Tasks", SchemaName.Tasks);

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property(p => p.Title)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(p => p.Descreption)
                .HasMaxLength(2000)
                .IsRequired();

            builder.Property(p => p.AssignedToUserId)
                .HasMaxLength(450)
                .IsRequired();

            builder.Property(p => p.StatusId)
                .IsRequired();

            builder.Property(p => p.CreatedBy)
                .HasMaxLength(450)
                .IsRequired();

            builder.Property(p => p.CreatedOn)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(p => p.ModifiedBy)
                .HasMaxLength(450)
                .IsRequired(false);

            builder.Property(p => p.ModifiedOn)
                .IsRequired(false);

            builder.Property(p => p.DeletedBy)
                .HasMaxLength(450)
                .IsRequired(false);

            builder.Property(p => p.DeletedOn)
                .IsRequired(false);

            builder.Property(p => p.IsDeleted)
                .IsRequired(false)
                .HasDefaultValue(false);

            builder.HasOne(t => t.AssignedToUser)
                .WithMany()
                .HasForeignKey(t => t.AssignedToUserId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Tasks_Users_AssignedToUserId");

            builder.HasOne(t => t.Status)
                .WithMany()
                .HasForeignKey(t => t.StatusId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Tasks_Lookups_StatusId");

            builder.HasIndex(x => x.AssignedToUserId)
                .HasDatabaseName("IX_Tasks_AssignedToUserId");

            builder.HasIndex(x => x.StatusId)
                .HasDatabaseName("IX_Tasks_StatusId");

            builder.HasIndex(x => x.Title)
                .HasDatabaseName("IX_Tasks_Title");

            builder.HasIndex(x => x.CreatedOn)
                .HasDatabaseName("IX_Tasks_CreatedOn");

            builder.HasIndex(x => x.IsDeleted)
                .HasDatabaseName("IX_Tasks_IsDeleted");

            builder.HasIndex(x => new { x.AssignedToUserId, x.StatusId })
                .HasDatabaseName("IX_Tasks_AssignedToUserId_StatusId");

            builder.HasIndex(x => new { x.StatusId, x.IsDeleted })
                .HasDatabaseName("IX_Tasks_StatusId_IsDeleted");

            builder.HasQueryFilter(x => x.IsDeleted == false || x.IsDeleted == null);
        }
    }
}
