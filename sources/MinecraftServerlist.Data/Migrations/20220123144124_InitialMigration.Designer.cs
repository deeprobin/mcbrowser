﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MinecraftServerlist.Data.Infrastructure;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MinecraftServerlist.Data.Migrations
{
    [DbContext(typeof(PostgresDbContext))]
    [Migration("20220123144124_InitialMigration")]
    partial class InitialMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.HasPostgresEnum(modelBuilder, "server_state", new[] { "default", "pending_activation", "enabled", "disabled_by_user", "disabled_due_to_s", "disabled_due_law" });
            NpgsqlModelBuilderExtensions.HasPostgresEnum(modelBuilder, "user_role", new[] { "default", "staff", "developer", "admin" });
            NpgsqlModelBuilderExtensions.HasPostgresEnum(modelBuilder, "user_state", new[] { "pending_email_verification", "enabled", "disabled_by_user", "disabled_due_to_s", "disabled_due_law" });
            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("MinecraftServerlist.Data.Entities.Server", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("now()");

                    b.Property<int>("OwnerId")
                        .HasColumnType("integer");

                    b.Property<string>("ServerAddress")
                        .IsRequired()
                        .IsUnicode(false)
                        .HasColumnType("text");

                    b.Property<int>("ServerPort")
                        .HasColumnType("integer");

                    b.Property<byte>("ServerState")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("smallint")
                        .HasDefaultValue((byte)0);

                    b.Property<string>("VotifierAddress")
                        .IsUnicode(false)
                        .HasColumnType("text");

                    b.Property<int?>("VotifierPort")
                        .HasColumnType("integer");

                    b.Property<string>("VotifierToken")
                        .IsUnicode(false)
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("OwnerId");

                    b.ToTable("ServerDbSet");
                });

            modelBuilder.Entity("MinecraftServerlist.Data.Entities.ServerDescription", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Culture")
                        .IsRequired()
                        .HasMaxLength(12)
                        .IsUnicode(false)
                        .HasColumnType("char(12)");

                    b.Property<string>("DiscordInvitationId")
                        .HasMaxLength(128)
                        .IsUnicode(false)
                        .HasColumnType("character varying(128)");

                    b.Property<string>("LongDescription")
                        .IsRequired()
                        .HasMaxLength(8192)
                        .IsUnicode(true)
                        .HasColumnType("character varying(8192)");

                    b.Property<int>("ServerId")
                        .HasColumnType("integer");

                    b.Property<string>("ShortDescription")
                        .IsRequired()
                        .HasMaxLength(512)
                        .IsUnicode(false)
                        .HasColumnType("character varying(512)");

                    b.Property<string>("TeamspeakAddress")
                        .HasMaxLength(256)
                        .IsUnicode(false)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(256)
                        .IsUnicode(false)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("Website")
                        .HasMaxLength(256)
                        .IsUnicode(false)
                        .HasColumnType("character varying(256)");

                    b.HasKey("Id");

                    b.HasIndex("ServerId");

                    b.ToTable("ServerDescriptionDbSet");
                });

            modelBuilder.Entity("MinecraftServerlist.Data.Entities.ServerPing", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("now()");

                    b.Property<int?>("MaxPlayers")
                        .HasColumnType("integer");

                    b.Property<string>("MessageOfTheDay")
                        .IsUnicode(true)
                        .HasColumnType("text");

                    b.Property<int?>("OnlinePlayers")
                        .HasColumnType("integer");

                    b.Property<int>("ServerId")
                        .HasColumnType("integer");

                    b.Property<string>("VersionName")
                        .IsUnicode(true)
                        .HasColumnType("text");

                    b.Property<int?>("VersionProtocol")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("ServerId");

                    b.HasIndex("CreatedAt", "Id", "ServerId");

                    b.ToTable("ServerPing");
                });

            modelBuilder.Entity("MinecraftServerlist.Data.Entities.ServerVoting", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("integer");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("now()");

                    b.Property<string>("MinecraftUsername")
                        .IsRequired()
                        .HasMaxLength(16)
                        .IsUnicode(false)
                        .HasColumnType("character varying(16)");

                    b.Property<int>("ServerId")
                        .HasColumnType("integer");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("ServerId");

                    b.HasIndex("CreatedAt", "Id", "ServerId");

                    b.ToTable("ServerVotingDbSet");
                });

            modelBuilder.Entity("MinecraftServerlist.Data.Entities.Session", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("now()");

                    b.Property<byte[]>("Ip")
                        .IsRequired()
                        .HasMaxLength(16)
                        .HasColumnType("bytea");

                    b.Property<int>("ParentUserId")
                        .HasColumnType("integer");

                    b.Property<bool>("Revoked")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasDefaultValue(false);

                    b.Property<byte[]>("TokenBytes")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("bytea");

                    b.Property<string>("UserAgent")
                        .IsRequired()
                        .HasMaxLength(4096)
                        .IsUnicode(true)
                        .HasColumnType("character varying(4096)");

                    b.Property<DateTime>("ValidUntil")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("ParentUserId");

                    b.ToTable("SessionDbSet");
                });

            modelBuilder.Entity("MinecraftServerlist.Data.Entities.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("now()");

                    b.Property<string>("DisplayName")
                        .IsRequired()
                        .HasMaxLength(128)
                        .IsUnicode(false)
                        .HasColumnType("character varying(128)");

                    b.Property<byte[]>("HashedPassword")
                        .IsRequired()
                        .HasMaxLength(512)
                        .HasColumnType("bytea");

                    b.Property<DateTime>("LastUpdatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("now()");

                    b.Property<string>("MailAddress")
                        .IsRequired()
                        .HasMaxLength(256)
                        .IsUnicode(false)
                        .HasColumnType("character varying(256)");

                    b.Property<byte>("UserState")
                        .HasColumnType("smallint");

                    b.HasKey("Id");

                    b.ToTable("UserDbSet");
                });

            modelBuilder.Entity("MinecraftServerlist.Data.Entities.Server", b =>
                {
                    b.HasOne("MinecraftServerlist.Data.Entities.User", "Owner")
                        .WithMany("Servers")
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Owner");
                });

            modelBuilder.Entity("MinecraftServerlist.Data.Entities.ServerDescription", b =>
                {
                    b.HasOne("MinecraftServerlist.Data.Entities.Server", "Server")
                        .WithMany("Descriptions")
                        .HasForeignKey("ServerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Server");
                });

            modelBuilder.Entity("MinecraftServerlist.Data.Entities.ServerPing", b =>
                {
                    b.HasOne("MinecraftServerlist.Data.Entities.Server", "Server")
                        .WithMany("ReceivedPings")
                        .HasForeignKey("ServerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Server");
                });

            modelBuilder.Entity("MinecraftServerlist.Data.Entities.ServerVoting", b =>
                {
                    b.HasOne("MinecraftServerlist.Data.Entities.Server", "Server")
                        .WithMany("ReceivedVotings")
                        .HasForeignKey("Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MinecraftServerlist.Data.Entities.User", "User")
                        .WithMany("SubmittedVotings")
                        .HasForeignKey("ServerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Server");

                    b.Navigation("User");
                });

            modelBuilder.Entity("MinecraftServerlist.Data.Entities.Session", b =>
                {
                    b.HasOne("MinecraftServerlist.Data.Entities.User", "User")
                        .WithMany("Sessions")
                        .HasForeignKey("ParentUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("MinecraftServerlist.Data.Entities.Server", b =>
                {
                    b.Navigation("Descriptions");

                    b.Navigation("ReceivedPings");

                    b.Navigation("ReceivedVotings");
                });

            modelBuilder.Entity("MinecraftServerlist.Data.Entities.User", b =>
                {
                    b.Navigation("Servers");

                    b.Navigation("Sessions");

                    b.Navigation("SubmittedVotings");
                });
#pragma warning restore 612, 618
        }
    }
}