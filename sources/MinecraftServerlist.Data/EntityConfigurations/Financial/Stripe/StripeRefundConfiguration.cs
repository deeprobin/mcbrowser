using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MinecraftServerlist.Data.Entities.Financial.Stripe;

namespace MinecraftServerlist.Data.EntityConfigurations.Financial.Stripe;

public sealed class StripeRefundConfiguration : IEntityTypeConfiguration<StripeRefund>
{
    public void Configure(EntityTypeBuilder<StripeRefund> builder)
    {
        builder
            .Property(entity => entity.Id)
            .IsRequired();

        builder.HasKey(entity => entity.Id);

        builder
            .Property(entity => entity.AmountInCents)
            .IsRequired();

        builder
            .Property(entity => entity.StripeId)
            // UTF-8 is not required
            .IsUnicode(false)
            .IsRequired();

        builder
            .Property(entity => entity.Description)
            .IsUnicode()
            .IsRequired(false);

        builder
            .Property(entity => entity.ChargeId)
            // UTF-8 is not required
            .IsUnicode(false)
            .IsRequired();

        builder
            .Property(entity => entity.PaymentIntentId)
            // UTF-8 is not required
            .IsUnicode(false)
            .IsRequired(false);

        builder
            .Property(entity => entity.CurrencyCode)
            .HasMaxLength(3)
            .IsFixedLength()
            .IsRequired();

        builder
            .Property(entity => entity.RefundReason)
            .IsRequired();

        builder
            .Property(entity => entity.RefundStatus)
            .IsConcurrencyToken()
            .IsRequired();

        builder
            .Property(entity => entity.UserId)
            .IsRequired();

        builder
            .HasOne(entity => entity.User)
            .WithMany(entity => entity.StripeRefunds);

        builder.HasIndex(entity => entity.StripeId);
        builder.HasIndex(entity => entity.PaymentIntentId);
    }
}