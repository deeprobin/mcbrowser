using Microsoft.EntityFrameworkCore;
using MinecraftServerlist.Data.Entities.Financial;
using MinecraftServerlist.Data.Entities.Financial.Stripe;
using MinecraftServerlist.Data.Entities.Identity;
using MinecraftServerlist.Data.Entities.Servers;
using MinecraftServerlist.Data.EntityConfigurations.Financial;
using MinecraftServerlist.Data.EntityConfigurations.Financial.Stripe;
using MinecraftServerlist.Data.EntityConfigurations.Identity;
using MinecraftServerlist.Data.EntityConfigurations.Servers;

namespace MinecraftServerlist.Data.Infrastructure;

public class PostgresDbContext : DbContext
{
    public PostgresDbContext(DbContextOptions<PostgresDbContext> options) : base(options)
    {
    }

    public virtual DbSet<Server> ServerDbSet { get; set; } = default!;

    public virtual DbSet<ServerDescription> ServerDescriptionDbSet { get; set; } = default!;

    public virtual DbSet<ServerPing> ServerPingDbSet { get; set; } = default!;

    public virtual DbSet<ServerVoting> ServerVotingDbSet { get; set; } = default!;

    public virtual DbSet<User> UserDbSet { get; set; } = default!;

    public virtual DbSet<Session> SessionDbSet { get; set; } = default!;

    #region Financial

    public virtual DbSet<Payment> PaymentDbSet { get; set; } = default!;

    public virtual DbSet<PaymentOperation> PaymentOperationDbSet { get; set; } = default!;

    public virtual DbSet<StripeRefund> StripeRefundDbSet { get; set; } = default!;

    public virtual DbSet<AutoVoteTask> AutoVoteTaskDbSet { get; set; } = default!;

    #endregion Financial

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            // Stub code for `dotnet ef migrations` tool
            optionsBuilder.UseNpgsql();
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // No domain
        modelBuilder.HasPostgresEnum<ServerState>();
        modelBuilder.HasPostgresEnum<UserState>();
        modelBuilder.HasPostgresEnum<UserRole>();

        modelBuilder.ApplyConfiguration(new ServerConfiguration());
        modelBuilder.ApplyConfiguration(new ServerDescriptionConfiguration());
        modelBuilder.ApplyConfiguration(new ServerPingConfiguration());
        modelBuilder.ApplyConfiguration(new ServerVotingConfiguration());
        modelBuilder.ApplyConfiguration(new SessionConfiguration());
        modelBuilder.ApplyConfiguration(new UserConfiguration());

        // Domain: Financial
        modelBuilder.HasPostgresEnum<StripeRefundReason>();
        modelBuilder.HasPostgresEnum<StripeRefundStatus>();
        modelBuilder.HasPostgresEnum<PaymentServiceProvider>();
        modelBuilder.HasPostgresEnum<PaymentOperationState>();

        modelBuilder.ApplyConfiguration(new StripePaymentConfiguration());
        modelBuilder.ApplyConfiguration(new StripeRefundConfiguration());
        modelBuilder.ApplyConfiguration(new AutoVoteTaskConfiguration());
        modelBuilder.ApplyConfiguration(new PaymentConfiguration());
        modelBuilder.ApplyConfiguration(new PaymentOperationConfiguration());
    }
}