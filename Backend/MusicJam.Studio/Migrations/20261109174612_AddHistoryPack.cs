using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MusicJam.Studio.Migrations
{
    /// <inheritdoc />
    public partial class AddHistoryPack : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_pack_round_rounds_round_id",
                table: "pack_round");

            migrationBuilder.DropTable(
                name: "games");

            migrationBuilder.DropTable(
                name: "statistics");

            migrationBuilder.CreateTable(
                name: "history_pack",
                columns: table => new
                {
                    pack_id = table.Column<Guid>(type: "uuid", nullable: false),
                    date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_history_pack", x => x.pack_id);
                    table.ForeignKey(
                        name: "fk_history_pack_packs_pack_id",
                        column: x => x.pack_id,
                        principalTable: "packs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.AddForeignKey(
                name: "fk_pack_round_round_round_id",
                table: "pack_round",
                column: "round_id",
                principalTable: "rounds",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_pack_round_round_round_id",
                table: "pack_round");

            migrationBuilder.DropTable(
                name: "history_pack");

            migrationBuilder.CreateTable(
                name: "games",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    pack_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    description = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    game_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    mode = table.Column<int>(type: "integer", nullable: false),
                    state = table.Column<int>(type: "integer", nullable: false),
                    title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_games", x => x.id);
                    table.ForeignKey(
                        name: "fk_games_packs_pack_id",
                        column: x => x.pack_id,
                        principalTable: "packs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_games_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "statistics",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    losses_count = table.Column<int>(type: "integer", nullable: false),
                    total_count = table.Column<int>(type: "integer", nullable: false),
                    wins_count = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_statistics", x => x.id);
                    table.ForeignKey(
                        name: "fk_statistics_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_games_pack_id",
                table: "games",
                column: "pack_id");

            migrationBuilder.CreateIndex(
                name: "ix_games_user_id",
                table: "games",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_statistics_user_id",
                table: "statistics",
                column: "user_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_pack_round_rounds_round_id",
                table: "pack_round",
                column: "round_id",
                principalTable: "rounds",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
