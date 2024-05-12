using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "tennis_schema");

            migrationBuilder.CreateTable(
                name: "tournaments",
                schema: "tennis_schema",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tournaments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                schema: "tennis_schema",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    Username = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Password = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    FirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    LastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    PhoneNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "matches",
                schema: "tennis_schema",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Score = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    TournamentId = table.Column<int>(type: "integer", nullable: false),
                    PlayerOneId = table.Column<int>(type: "integer", nullable: false),
                    PlayerTwoId = table.Column<int>(type: "integer", nullable: false),
                    RefereeId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_matches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_matches_tournaments_TournamentId",
                        column: x => x.TournamentId,
                        principalSchema: "tennis_schema",
                        principalTable: "tournaments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_matches_users_PlayerOneId",
                        column: x => x.PlayerOneId,
                        principalSchema: "tennis_schema",
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_matches_users_PlayerTwoId",
                        column: x => x.PlayerTwoId,
                        principalSchema: "tennis_schema",
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_matches_users_RefereeId",
                        column: x => x.RefereeId,
                        principalSchema: "tennis_schema",
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tournament_participants",
                schema: "tennis_schema",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TournamentId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tournament_participants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tournament_participants_tournaments_TournamentId",
                        column: x => x.TournamentId,
                        principalSchema: "tennis_schema",
                        principalTable: "tournaments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tournament_participants_users_UserId",
                        column: x => x.UserId,
                        principalSchema: "tennis_schema",
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_matches_PlayerOneId",
                schema: "tennis_schema",
                table: "matches",
                column: "PlayerOneId");

            migrationBuilder.CreateIndex(
                name: "IX_matches_PlayerTwoId",
                schema: "tennis_schema",
                table: "matches",
                column: "PlayerTwoId");

            migrationBuilder.CreateIndex(
                name: "IX_matches_RefereeId",
                schema: "tennis_schema",
                table: "matches",
                column: "RefereeId");

            migrationBuilder.CreateIndex(
                name: "IX_matches_TournamentId",
                schema: "tennis_schema",
                table: "matches",
                column: "TournamentId");

            migrationBuilder.CreateIndex(
                name: "IX_tournament_participants_TournamentId",
                schema: "tennis_schema",
                table: "tournament_participants",
                column: "TournamentId");

            migrationBuilder.CreateIndex(
                name: "IX_tournament_participants_UserId",
                schema: "tennis_schema",
                table: "tournament_participants",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_users_Username",
                schema: "tennis_schema",
                table: "users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "matches",
                schema: "tennis_schema");

            migrationBuilder.DropTable(
                name: "tournament_participants",
                schema: "tennis_schema");

            migrationBuilder.DropTable(
                name: "tournaments",
                schema: "tennis_schema");

            migrationBuilder.DropTable(
                name: "users",
                schema: "tennis_schema");
        }
    }
}
