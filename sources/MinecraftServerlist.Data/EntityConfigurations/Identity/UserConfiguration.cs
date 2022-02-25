using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MinecraftServerlist.Data.Entities.Identity;

namespace MinecraftServerlist.Data.EntityConfigurations.Identity;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder
            .Property(entity => entity.Id)
            .IsRequired();

        builder.HasKey(entity => entity.Id);

        builder.Property(entity => entity.DisplayName)
            .IsRequired()
            .HasMaxLength(128)
            // UTF-8 not required
            .IsUnicode(false);

        builder.Property(entity => entity.MailAddress)
            .IsRequired()
            .HasMaxLength(256)
            // UTF-8 not required
            .IsUnicode(false);

        builder.Property(entity => entity.HashedPassword)
            .IsRequired()
            .HasColumnType("bytea")
            .HasMaxLength(512);

        builder.Property(entity => entity.UserRole)
            .IsRequired();

        builder.Property(entity => entity.UserState)
            .IsRequired();

        builder.HasMany(entity => entity.Servers)
            .WithOne(entity => entity.Owner)
            .HasForeignKey(entity => entity.OwnerId);

        builder.HasMany(entity => entity.Sessions)
            .WithOne(entity => entity.User)
            .HasForeignKey(entity => entity.ParentUserId);

        builder.HasMany(entity => entity.SubmittedVotings)
            .WithOne(entity => entity.User)
            .HasForeignKey(entity => entity.ServerId);

        builder.HasMany(entity => entity.StripeRefunds)
            .WithOne(entity => entity.User)
            .HasForeignKey(entity => entity.UserId);

        builder.Property(entity => entity.CreatedAt)
            // https://www.postgresql.org/docs/10/functions-datetime.html
            .HasDefaultValueSql("now()")
            .IsRequired();

        builder.Property(entity => entity.LastUpdatedAt)
            // https://www.postgresql.org/docs/10/functions-datetime.html
            .HasDefaultValueSql("now()")
            .IsRequired();
    }
}