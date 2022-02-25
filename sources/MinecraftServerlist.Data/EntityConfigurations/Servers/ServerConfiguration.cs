using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MinecraftServerlist.Data.Entities.Servers;

namespace MinecraftServerlist.Data.EntityConfigurations.Servers;

public sealed class ServerConfiguration : IEntityTypeConfiguration<Server>
{
    public void Configure(EntityTypeBuilder<Server> builder)
    {
        builder
            .Property(entity => entity.Id)
            .IsRequired();

        builder.HasKey(entity => entity.Id);

        builder.Property(entity => entity.ServerAddress)
            .IsRequired()
            // UTF-8 not required
            .IsUnicode(false);

        builder.Property(entity => entity.ServerPort)
            .IsRequired();

        builder.Property(entity => entity.ServerState)
            .HasDefaultValue(ServerState.Default)
            .IsRequired();

        builder.HasMany(entity => entity.Descriptions)
            .WithOne(entity => entity.Server)
            .HasForeignKey(entity => entity.ServerId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder.HasOne(entity => entity.Owner)
            .WithMany(entity => entity.Servers)
            .HasForeignKey(entity => entity.OwnerId);

        builder.HasMany(entity => entity.ReceivedVotings)
            .WithOne(entity => entity.Server)
            .HasForeignKey(entity => entity.Id);

        builder.HasMany(entity => entity.ReceivedPings)
            .WithOne(entity => entity.Server)
            .HasForeignKey(entity => entity.Id);

        builder.Property(entity => entity.CreatedAt)
            // https://www.postgresql.org/docs/10/functions-datetime.html
            .HasDefaultValueSql("now()")
            .IsRequired();

        builder.Property(entity => entity.VotifierAddress)
            .IsRequired(false)
            // UTF-8 not required
            .IsUnicode(false);

        builder.Property(entity => entity.VotifierPort)
            .IsRequired(false);

        builder.Property(entity => entity.VotifierToken)
            .IsRequired(false)
            // UTF-8 AFAIK not required
            .IsUnicode(false);
    }
}