using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMS.API.Migrations
{
    /// <inheritdoc />
    public partial class updateDoubtReplies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DoubtReplies_Users_UserId",
                table: "DoubtReplies");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "DoubtReplies",
                newName: "TeacherId");

            migrationBuilder.RenameIndex(
                name: "IX_DoubtReplies_UserId",
                table: "DoubtReplies",
                newName: "IX_DoubtReplies_TeacherId");

            migrationBuilder.AddForeignKey(
                name: "FK_DoubtReplies_Users_TeacherId",
                table: "DoubtReplies",
                column: "TeacherId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DoubtReplies_Users_TeacherId",
                table: "DoubtReplies");

            migrationBuilder.RenameColumn(
                name: "TeacherId",
                table: "DoubtReplies",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_DoubtReplies_TeacherId",
                table: "DoubtReplies",
                newName: "IX_DoubtReplies_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_DoubtReplies_Users_UserId",
                table: "DoubtReplies",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
