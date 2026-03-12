using AdvanceRequests.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdvanceRequests.Infrastructure.Persistence.Configurations;

public sealed class AdvanceRequestStatusHistoryConfiguration : IEntityTypeConfiguration<AdvanceRequestStatusHistory>
{
    public void Configure(EntityTypeBuilder<AdvanceRequestStatusHistory> builder)
    {
        builder.ToTable("advance_request_status_history");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.AdvanceRequestId)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.ChangedBy)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.ChangedAtUtc)
            .IsRequired();

        builder.HasIndex(x => x.AdvanceRequestId)
            .HasDatabaseName("ix_advance_request_status_history_request_id");
    }
}