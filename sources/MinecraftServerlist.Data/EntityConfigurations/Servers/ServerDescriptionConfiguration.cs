using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MinecraftServerlist.Data.Entities.Servers;

namespace MinecraftServerlist.Data.EntityConfigurations.Servers;

public sealed class ServerDescriptionConfiguration : IEntityTypeConfiguration<ServerDescription>
{
    public void Configure(EntityTypeBuilder<ServerDescription> builder)
    {
        builder
            .Property(entity => entity.Id)
            .IsRequired();

        builder.HasKey(entity => entity.Id);

        builder.HasOne(entity => entity.Server)
            .WithMany(entity => entity.Descriptions)
            .HasForeignKey(entity => entity.ServerId)
            .IsRequired();

        builder.Property(entity => entity.Culture)
            .HasColumnType("char")
            .IsRequired()
            .IsUnicode(false)
            .HasMaxLength(12);

        builder.Property(entity => entity.Title)
            .HasMaxLength(256)
            // UTF-8 not required
            .IsUnicode(false)
            .IsRequired();

        builder.Property(entity => entity.ShortDescription)
            .HasMaxLength(512)
            // UTF-8 not required
            .IsUnicode(false)
            .IsRequired();

        builder.Property(entity => entity.LongDescription)
            .HasMaxLength(8192)
            // UTF-8 is required
            .IsUnicode()
            .IsRequired();

        builder.Property(entity => entity.DiscordInvitationId)
            .HasMaxLength(128)
            // UTF-8 not required
            .IsUnicode(false);

        builder.Property(entity => entity.TeamspeakAddress)
            .HasMaxLength(256)
            // UTF-8 not required
            .IsUnicode(false);

        builder.Property(entity => entity.Website)
            .HasMaxLength(256)
            // UTF-8 not required
            .IsUnicode(false);
    }
}