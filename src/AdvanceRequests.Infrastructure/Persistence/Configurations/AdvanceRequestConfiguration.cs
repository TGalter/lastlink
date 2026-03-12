using AdvanceRequests.Domain.Entities;
using AdvanceRequests.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdvanceRequests.Infrastructure.Persistence.Configurations;

public sealed class AdvanceRequestConfiguration : IEntityTypeConfiguration<AdvanceRequest>
{
    public void Configure(EntityTypeBuilder<AdvanceRequest> builder)
    {
        builder.ToTable("advance_requests");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.CreatorId)
            .IsRequired();

        builder.Property(x => x.GrossAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.FeePercentageApplied)
            .HasPrecision(5, 4)
            .IsRequired();

        builder.Property(x => x.FeeAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.NetAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.RequestedAtUtc)
            .IsRequired();

        builder.Property(x => x.ProcessedAtUtc);

        builder.Ignore(x => x.DomainEvents);

        builder.HasMany(x => x.StatusHistory)
        .WithOne()
        .HasForeignKey(x => x.AdvanceRequestId)
        .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.CreatorId)
            .HasDatabaseName("ix_advance_requests_creator_id");

        builder.HasIndex(x => x.Status)
            .HasDatabaseName("ix_advance_requests_status");

        builder.HasIndex(x => x.CreatorId)
            .HasDatabaseName("ux_advance_requests_creator_pending")
            .HasFilter($"\"Status\" = {(int)AdvanceRequestStatus.Pending}")
            .IsUnique();
    }
}