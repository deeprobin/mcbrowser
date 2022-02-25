using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MinecraftServerlist.Data.Entities.Financial;

namespace MinecraftServerlist.Data.EntityConfigurations.Financial;

public sealed class AutoVoteTaskConfiguration : IEntityTypeConfiguration<AutoVoteTask>
{
    public void Configure(EntityTypeBuilder<AutoVoteTask> builder)
    {
        builder
            .Property(entity => entity.Id)
            .IsRequired();

        builder.HasKey(entity => entity.Id);

        builder
            .Property(entity => entity.StartDate)
            .HasConversion(
                dateOnly => new DateTime(dateOnly.Year, dateOnly.Month, dateOnly.Day, 0, 0, 0, DateTimeKind.Unspecified),
                dateTime => DateOnly.FromDateTime(dateTime))
            .IsRequired();

        builder
            .Property(entity => entity.EndDate)
            .HasConversion(
                dateOnly => new DateTime(dateOnly.Year, dateOnly.Month, dateOnly.Day, 0, 0, 0, DateTimeKind.Unspecified),
                dateTime => DateOnly.FromDateTime(dateTime))
            .IsRequired();

        builder
            .Property(entity => entity.TargetServerId)
            .IsRequired();

        builder
            .Property(entity => entity.TargetUuid)
            .HasMaxLength(16)
            .IsFixedLength()
            .IsRequired();

        builder
            .Property(entity => entity.PaymentId)
            .IsRequired(false);

        builder
            .HasOne(entity => entity.TargetServer)
            .WithMany(entity => entity.AutoVoteTasks);

        builder
            .HasOne(entity => entity.Payment)
            .WithMany(entity => entity.AutoVoteTasks);
    }
}