using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMS.API.Migrations
{
    /// <inheritdoc />
    public partial class logourlupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "LogoUrl", "MinPasswordLength" },
                values: new object[] { "https://www.bing.com/images/search?q=logo.jpg&FORM=IQFRBA&id=5CE7D99227F3B6773B0026254E307A3A89034874", 9 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "LogoUrl", "MinPasswordLength" },
                values: new object[] { "https://example.com/default-logo.png", 6 });
        }
    }
}
