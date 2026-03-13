using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RedZone.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUserToPrediction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Predictions",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Predictions_UserId",
                table: "Predictions",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Predictions_AspNetUsers_UserId",
                table: "Predictions",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Predictions_AspNetUsers_UserId",
                table: "Predictions");

            migrationBuilder.DropIndex(
                name: "IX_Predictions_UserId",
                table: "Predictions");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Predictions");
        }
    }
}
