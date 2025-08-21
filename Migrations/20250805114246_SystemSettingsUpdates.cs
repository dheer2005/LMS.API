using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMS.API.Migrations
{
    /// <inheritdoc />
    public partial class SystemSettingsUpdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Value",
                table: "SystemSettings",
                newName: "WelcomeMessage");

            migrationBuilder.RenameColumn(
                name: "Key",
                table: "SystemSettings",
                newName: "Theme");

            migrationBuilder.AddColumn<string>(
                name: "AllowedVideoFormats",
                table: "SystemSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "ChatEnabled",
                table: "SystemSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "DefaultQuizTimeMinutes",
                table: "SystemSettings",
                type: "int",
                nullable: false,
                defaultValue: 0);

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

            migrationBuilder.AddColumn<DateTime>(
                name: "LastBackupDate",
                table: "SystemSettings",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LogoUrl",
                table: "SystemSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "MaxVideoUploadSizeMB",
                table: "SystemSettings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MinPasswordLength",
                table: "SystemSettings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "PaidCoursesEnabled",
                table: "SystemSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "PassPercentage",
                table: "SystemSettings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "PlatformName",
                table: "SystemSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "QuizRetakeLimit",
                table: "SystemSettings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "RazorpayKey",
                table: "SystemSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "RegistrationEnabled",
                table: "SystemSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "RequireEmailVerification",
                table: "SystemSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "RequireEnrollmentApproval",
                table: "SystemSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "SessionTimeoutMinutes",
                table: "SystemSettings",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllowedVideoFormats",
                table: "SystemSettings");

            migrationBuilder.DropColumn(
                name: "ChatEnabled",
                table: "SystemSettings");

            migrationBuilder.DropColumn(
                name: "DefaultQuizTimeMinutes",
                table: "SystemSettings");

            migrationBuilder.DropColumn(
                name: "EnableUserActivityLog",
                table: "SystemSettings");

            migrationBuilder.DropColumn(
                name: "GoogleAnalyticsCode",
                table: "SystemSettings");

            migrationBuilder.DropColumn(
                name: "LastBackupDate",
                table: "SystemSettings");

            migrationBuilder.DropColumn(
                name: "LogoUrl",
                table: "SystemSettings");

            migrationBuilder.DropColumn(
                name: "MaxVideoUploadSizeMB",
                table: "SystemSettings");

            migrationBuilder.DropColumn(
                name: "MinPasswordLength",
                table: "SystemSettings");

            migrationBuilder.DropColumn(
                name: "PaidCoursesEnabled",
                table: "SystemSettings");

            migrationBuilder.DropColumn(
                name: "PassPercentage",
                table: "SystemSettings");

            migrationBuilder.DropColumn(
                name: "PlatformName",
                table: "SystemSettings");

            migrationBuilder.DropColumn(
                name: "QuizRetakeLimit",
                table: "SystemSettings");

            migrationBuilder.DropColumn(
                name: "RazorpayKey",
                table: "SystemSettings");

            migrationBuilder.DropColumn(
                name: "RegistrationEnabled",
                table: "SystemSettings");

            migrationBuilder.DropColumn(
                name: "RequireEmailVerification",
                table: "SystemSettings");

            migrationBuilder.DropColumn(
                name: "RequireEnrollmentApproval",
                table: "SystemSettings");

            migrationBuilder.DropColumn(
                name: "SessionTimeoutMinutes",
                table: "SystemSettings");

            migrationBuilder.RenameColumn(
                name: "WelcomeMessage",
                table: "SystemSettings",
                newName: "Value");

            migrationBuilder.RenameColumn(
                name: "Theme",
                table: "SystemSettings",
                newName: "Key");
        }
    }
}
