using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MinecraftServerlist.Data.Entities.Identity;

namespace MinecraftServerlist.Data.EntityConfigurations.Identity;

public sealed class SessionConfiguration : IEntityTypeConfiguration<Session>
{
    public void Configure(EntityTypeBuilder<Session> builder)
    {
        builder
            .Property(entity => entity.Id)
            .IsRequired();

        builder.HasKey(entity => entity.Id);

        builder.Property(entity => entity.Ip)
            .HasColumnType("bytea")
            // Ipv4 = 4 bytes
            // Ipv6 = 16 bytes
            .HasMaxLength(16)
            .IsRequired();

        builder.HasOne(entity => entity.User)
            .WithMany(entity => entity.Sessions)
            .HasForeignKey(entity => entity.ParentUserId);

        builder.Property(entity => entity.TokenBytes)
            .HasColumnType("bytea")
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(entity => entity.Revoked)
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(entity => entity.UserAgent)
            .IsUnicode()
            .HasMaxLength(4096);

        builder.Property(entity => entity.ValidUntil)
            .IsRequired();

        builder.Property(entity => entity.CreatedAt)
            .HasDefaultValueSql("now()")
            .IsRequired();
    }
}