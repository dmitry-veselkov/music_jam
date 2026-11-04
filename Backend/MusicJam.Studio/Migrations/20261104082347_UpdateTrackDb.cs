using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MusicJam.Studio.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTrackDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "link",
                table: "tracks");

            migrationBuilder.AddColumn<Guid>(
                name: "object_key",
                table: "tracks",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "object_key",
                table: "tracks");

            migrationBuilder.AddColumn<string>(
                name: "link",
                table: "tracks",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
