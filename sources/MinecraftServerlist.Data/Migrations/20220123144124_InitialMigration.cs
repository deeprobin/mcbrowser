using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MinecraftServerlist.Data.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:server_state", "default,pending_activation,enabled,disabled_by_user,disabled_due_to_s,disabled_due_law")
                .Annotation("Npgsql:Enum:user_role", "default,staff,developer,admin")
                .Annotation("Npgsql:Enum:user_state", "pending_email_verification,enabled,disabled_by_user,disabled_due_to_s,disabled_due_law");

            migrationBuilder.CreateTable(
                name: "UserDbSet",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MailAddress = table.Column<string>(type: "character varying(256)", unicode: false, maxLength: 256, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(128)", unicode: false, maxLength: 128, nullable: false),
                    HashedPassword = table.Column<byte[]>(type: "bytea", maxLength: 512, nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    UserState = table.Column<byte>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDbSet", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServerDbSet",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OwnerId = table.Column<int>(type: "integer", nullable: false),
                    ServerAddress = table.Column<string>(type: "text", unicode: false, nullable: false),
                    ServerPort = table.Column<int>(type: "integer", nullable: false),
                    ServerState = table.Column<byte>(type: "smallint", nullable: false, defaultValue: (byte)0),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    VotifierAddress = table.Column<string>(type: "text", unicode: false, nullable: true),
                    VotifierPort = table.Column<int>(type: "integer", nullable: true),
                    VotifierToken = table.Column<string>(type: "text", unicode: false, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServerDbSet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServerDbSet_UserDbSet_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "UserDbSet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SessionDbSet",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ParentUserId = table.Column<int>(type: "integer", nullable: false),
                    Ip = table.Column<byte[]>(type: "bytea", maxLength: 16, nullable: false),
                    TokenBytes = table.Column<byte[]>(type: "bytea", maxLength: 128, nullable: false),
                    UserAgent = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: false),
                    Revoked = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    ValidUntil = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SessionDbSet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SessionDbSet_UserDbSet_ParentUserId",
                        column: x => x.ParentUserId,
                        principalTable: "UserDbSet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServerDescriptionDbSet",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ServerId = table.Column<int>(type: "integer", nullable: false),
                    Culture = table.Column<string>(type: "char(12)", unicode: false, maxLength: 12, nullable: false),
                    Title = table.Column<string>(type: "character varying(256)", unicode: false, maxLength: 256, nullable: false),
                    ShortDescription = table.Column<string>(type: "character varying(512)", unicode: false, maxLength: 512, nullable: false),
                    LongDescription = table.Column<string>(type: "character varying(8192)", maxLength: 8192, nullable: false),
                    Website = table.Column<string>(type: "character varying(256)", unicode: false, maxLength: 256, nullable: true),
                    DiscordInvitationId = table.Column<string>(type: "character varying(128)", unicode: false, maxLength: 128, nullable: true),
                    TeamspeakAddress = table.Column<string>(type: "character varying(256)", unicode: false, maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServerDescriptionDbSet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServerDescriptionDbSet_ServerDbSet_ServerId",
                        column: x => x.ServerId,
                        principalTable: "ServerDbSet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServerPing",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ServerId = table.Column<int>(type: "integer", nullable: false),
                    MessageOfTheDay = table.Column<string>(type: "text", nullable: true),
                    OnlinePlayers = table.Column<int>(type: "integer", nullable: true),
                    MaxPlayers = table.Column<int>(type: "integer", nullable: true),
                    VersionName = table.Column<string>(type: "text", nullable: true),
                    VersionProtocol = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServerPing", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServerPing_ServerDbSet_ServerId",
                        column: x => x.ServerId,
                        principalTable: "ServerDbSet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServerVotingDbSet",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    ServerId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    MinecraftUsername = table.Column<string>(type: "character varying(16)", unicode: false, maxLength: 16, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServerVotingDbSet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServerVotingDbSet_ServerDbSet_Id",
                        column: x => x.Id,
                        principalTable: "ServerDbSet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServerVotingDbSet_UserDbSet_ServerId",
                        column: x => x.ServerId,
                        principalTable: "UserDbSet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServerDbSet_OwnerId",
                table: "ServerDbSet",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_ServerDescriptionDbSet_ServerId",
                table: "ServerDescriptionDbSet",
                column: "ServerId");

            migrationBuilder.CreateIndex(
                name: "IX_ServerPing_CreatedAt_Id_ServerId",
                table: "ServerPing",
                columns: new[] { "CreatedAt", "Id", "ServerId" });

            migrationBuilder.CreateIndex(
                name: "IX_ServerPing_ServerId",
                table: "ServerPing",
                column: "ServerId");

            migrationBuilder.CreateIndex(
                name: "IX_ServerVotingDbSet_CreatedAt_Id_ServerId",
                table: "ServerVotingDbSet",
                columns: new[] { "CreatedAt", "Id", "ServerId" });

            migrationBuilder.CreateIndex(
                name: "IX_ServerVotingDbSet_ServerId",
                table: "ServerVotingDbSet",
                column: "ServerId");

            migrationBuilder.CreateIndex(
                name: "IX_SessionDbSet_ParentUserId",
                table: "SessionDbSet",
                column: "ParentUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ServerDescriptionDbSet");

            migrationBuilder.DropTable(
                name: "ServerPing");

            migrationBuilder.DropTable(
                name: "ServerVotingDbSet");

            migrationBuilder.DropTable(
                name: "SessionDbSet");

            migrationBuilder.DropTable(
                name: "ServerDbSet");

            migrationBuilder.DropTable(
                name: "UserDbSet");
        }
    }
}