using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MinecraftServerlist.Data.Entities.Financial.Stripe;

namespace MinecraftServerlist.Data.EntityConfigurations.Financial.Stripe;

public sealed class StripePaymentConfiguration : IEntityTypeConfiguration<StripePayment>
{
    public void Configure(EntityTypeBuilder<StripePayment> builder)
    {
        builder.Property(entity => entity.StripeId)
            .IsRequired()
            .IsUnicode(false)
            // TODO: Find right length (stripe specs)
            .HasMaxLength(256);

        builder
            .HasIndex(entity => entity.StripeId)
            .IsUnique();
    }
}