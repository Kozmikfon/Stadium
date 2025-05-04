using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stadyum.API.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdToPlayer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Players",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Players");
        }
    }
}
