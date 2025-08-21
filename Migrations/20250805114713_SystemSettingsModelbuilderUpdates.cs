using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMS.API.Migrations
{
    /// <inheritdoc />
    public partial class SystemSettingsModelbuilderUpdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "SystemSettings",
                columns: new[] { "Id", "AllowedVideoFormats", "ChatEnabled", "DefaultQuizTimeMinutes", "EnableUserActivityLog", "GoogleAnalyticsCode", "LastBackupDate", "LogoUrl", "MaxVideoUploadSizeMB", "MinPasswordLength", "PaidCoursesEnabled", "PassPercentage", "PlatformName", "QuizRetakeLimit", "RazorpayKey", "RegistrationEnabled", "RequireEmailVerification", "RequireEnrollmentApproval", "SessionTimeoutMinutes", "Theme", "WelcomeMessage" },
                values: new object[] { 1, "mp4,webm,mov", true, 30, false, "", null, "https://example.com/default-logo.png", 500, 6, false, 50, "Smart LMS", 2, "", true, false, false, 30, "light", "Welcome to our LMS!" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
