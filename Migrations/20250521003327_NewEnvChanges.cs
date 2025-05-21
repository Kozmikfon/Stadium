using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stadyum.API.Migrations
{
    /// <inheritdoc />
    public partial class NewEnvChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsInTournament",
                table: "Teams",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsInTournament",
                table: "Teams");
        }
    }
}
