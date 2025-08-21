using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMS.API.Migrations
{
    /// <inheritdoc />
    public partial class Systemsettingupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChatEnabled",
                table: "SystemSettings");

            migrationBuilder.DropColumn(
                name: "EnableUserActivityLog",
                table: "SystemSettings");

            migrationBuilder.DropColumn(
                name: "GoogleAnalyticsCode",
                table: "SystemSettings");

            migrationBuilder.DropColumn(
                name: "RazorpayKey",
                table: "SystemSettings");

            migrationBuilder.DropColumn(
                name: "WelcomeMessage",
                table: "SystemSettings");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ChatEnabled",
                table: "SystemSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "EnableUserActivityLog",
                table: "SystemSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "GoogleAnalyticsCode",
                table: "SystemSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RazorpayKey",
                table: "SystemSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "WelcomeMessage",
                table: "SystemSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ChatEnabled", "EnableUserActivityLog", "GoogleAnalyticsCode", "RazorpayKey", "WelcomeMessage" },
                values: new object[] { true, false, "", "", "Welcome to our LMS!" });
        }
    }
}
