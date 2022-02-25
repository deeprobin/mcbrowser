using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MinecraftServerlist.Data.Entities.Servers;

namespace MinecraftServerlist.Data.EntityConfigurations.Servers;

public sealed class ServerPingConfiguration : IEntityTypeConfiguration<ServerPing>
{
    public void Configure(EntityTypeBuilder<ServerPing> builder)
    {
        builder
            .Property(entity => entity.Id)
            .IsRequired();

        builder.HasKey(entity => entity.Id);

        builder.Property(entity => entity.ServerId)
            .IsRequired();

        builder.Property(entity => entity.MessageOfTheDay)
            .IsUnicode()
            .IsRequired(false);

        builder.Property(entity => entity.OnlinePlayers)
            .IsRequired(false);

        builder.Property(entity => entity.MaxPlayers)
            .IsRequired(false);

        builder.Property(entity => entity.VersionName)
            .IsUnicode()
            .IsRequired(false);

        builder.Property(entity => entity.VersionProtocol)
            .IsRequired(false);

        builder.Property(entity => entity.Online)
            .IsRequired();

        builder.HasOne(entity => entity.Server)
            .WithMany(entity => entity.ReceivedPings)
            .HasForeignKey(entity => entity.ServerId);

        builder.Property(entity => entity.CreatedAt)
            // https://www.postgresql.org/docs/10/functions-datetime.html
            .HasDefaultValueSql("now()")
            .IsRequired();

        // This index is important because we frequently select pings in a specific time range
        builder.HasIndex(entity => new
        {
            entity.CreatedAt,
            entity.Id,
            entity.ServerId
        });
    }
}