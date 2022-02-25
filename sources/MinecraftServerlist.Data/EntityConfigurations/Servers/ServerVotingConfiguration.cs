using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MinecraftServerlist.Data.Entities.Servers;

namespace MinecraftServerlist.Data.EntityConfigurations.Servers;

public sealed class ServerVotingConfiguration : IEntityTypeConfiguration<ServerVoting>
{
    public void Configure(EntityTypeBuilder<ServerVoting> builder)
    {
        builder
            .Property(entity => entity.Id)
            .IsRequired();

        builder.HasKey(entity => entity.Id);

        builder.Property(entity => entity.ServerId)
            .IsRequired();

        builder.Property(entity => entity.UserId)
            .IsRequired();

        builder.Property(entity => entity.MinecraftUsername)
            .IsUnicode(false)
            .HasMaxLength(16)
            .IsRequired();

        builder.HasOne(entity => entity.User)
            .WithMany(entity => entity.SubmittedVotings)
            .HasForeignKey(entity => entity.Id);

        builder.HasOne(entity => entity.Server)
            .WithMany(entity => entity.ReceivedVotings)
            .HasForeignKey(entity => entity.Id);

        builder.Property(entity => entity.CreatedAt)
            // https://www.postgresql.org/docs/10/functions-datetime.html
            .HasDefaultValueSql("now()")
            .IsRequired();

        // This index is important because we frequently group all votings in a specific time range
        builder.HasIndex(entity => new
        {
            entity.CreatedAt,
            entity.Id,
            entity.ServerId
        });
    }
}