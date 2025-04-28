using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stadyum.API.Migrations
{
    /// <inheritdoc />
    public partial class FixPlayerCreateAdWithTimezone : Migration
    {

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
    name: "CreateAd",
     table: "Players",
    type: "timestamp with time zone",
    nullable: false,
    oldClrType: typeof(DateTime),
    oldType: "timestamp without time zone");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "TeamMembers",
                newName: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMembers_PlayerId",
                table: "TeamMembers",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMembers_TeamId",
                table: "TeamMembers",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Offers_MatchId",
                table: "Offers",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_Offers_ReceiverId",
                table: "Offers",
                column: "ReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_Offers_SenderId",
                table: "Offers",
                column: "SenderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Offers_Matches_MatchId",
                table: "Offers",
                column: "MatchId",
                principalTable: "Matches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Offers_Players_ReceiverId",
                table: "Offers",
                column: "ReceiverId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Offers_Players_SenderId",
                table: "Offers",
                column: "SenderId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TeamMembers_Players_PlayerId",
                table: "TeamMembers",
                column: "PlayerId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TeamMembers_Teams_TeamId",
                table: "TeamMembers",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
    name: "CreateAd",
    table: "Players",
    type: "timestamp without time zone",
    nullable: false,
    oldClrType: typeof(DateTime),
    oldType: "timestamp with time zone");

            migrationBuilder.DropForeignKey(
                name: "FK_Offers_Matches_MatchId",
                table: "Offers");

            migrationBuilder.DropForeignKey(
                name: "FK_Offers_Players_ReceiverId",
                table: "Offers");

            migrationBuilder.DropForeignKey(
                name: "FK_Offers_Players_SenderId",
                table: "Offers");

            migrationBuilder.DropForeignKey(
                name: "FK_TeamMembers_Players_PlayerId",
                table: "TeamMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_TeamMembers_Teams_TeamId",
                table: "TeamMembers");

            migrationBuilder.DropIndex(
                name: "IX_TeamMembers_PlayerId",
                table: "TeamMembers");

            migrationBuilder.DropIndex(
                name: "IX_TeamMembers_TeamId",
                table: "TeamMembers");

            migrationBuilder.DropIndex(
                name: "IX_Offers_MatchId",
                table: "Offers");

            migrationBuilder.DropIndex(
                name: "IX_Offers_ReceiverId",
                table: "Offers");

            migrationBuilder.DropIndex(
                name: "IX_Offers_SenderId",
                table: "Offers");

            migrationBuilder.RenameColumn(
                name: "PlayerId",
                table: "TeamMembers",
                newName: "UserId");
        }
    }
}
