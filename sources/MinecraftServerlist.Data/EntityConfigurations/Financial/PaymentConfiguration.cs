using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MinecraftServerlist.Data.Entities.Financial;
using MinecraftServerlist.Data.Entities.Financial.Stripe;

namespace MinecraftServerlist.Data.EntityConfigurations.Financial;

internal sealed class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.Property(entity => entity.Id)
            .IsRequired();

        builder.HasKey(entity => entity.Id);

        builder.HasDiscriminator<PaymentServiceProvider>("PaymentServiceProvider")
            .HasValue<StripePayment>(PaymentServiceProvider.Stripe)
            .HasValue<Payment>(PaymentServiceProvider.None);

        builder.HasMany(entity => entity.AutoVoteTasks)
            .WithOne(entity => entity.Payment)
            .HasForeignKey(entity => entity.PaymentId);

        builder.Property(entity => entity.PaymentOperationId)
            .IsRequired(false);

        builder.HasOne(entity => entity.PaymentOperation)
            .WithOne(entity => entity.Payment);
    }
}