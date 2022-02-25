using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MinecraftServerlist.Data.Entities.Financial;

namespace MinecraftServerlist.Data.EntityConfigurations.Financial;

public sealed class PaymentOperationConfiguration : IEntityTypeConfiguration<PaymentOperation>
{
    public void Configure(EntityTypeBuilder<PaymentOperation> builder)
    {
        builder.Property(entity => entity.Id)
            .IsRequired();

        builder.HasKey(entity => entity.Id);

        builder.Property(entity => entity.State)
            .IsRequired()
            .IsConcurrencyToken();

        builder.Property(entity => entity.PaymentId)
            .IsRequired(false);

        builder.Property(entity => entity.CreatedAt)
            .HasDefaultValueSql("now()")
            .IsRequired();

        builder.HasOne(entity => entity.Payment)
            .WithOne(entity => entity.PaymentOperation);
    }
}