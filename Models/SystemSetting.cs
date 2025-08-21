namespace LMS.API.Models
{
    public class SystemSetting
    {
        public int Id { get; set; }
        public string PlatformName { get; set; }
        public string LogoUrl { get; set; }

        public int MaxVideoUploadSizeMB { get; set; }
        public string AllowedVideoFormats { get; set; } 
        public int QuizRetakeLimit { get; set; }
        public bool RegistrationEnabled { get; set; }
        public bool RequireEmailVerification { get; set; }
        public int MinPasswordLength { get; set; }
        public int SessionTimeoutMinutes { get; set; }
        public bool PaidCoursesEnabled { get; set; }
        public DateTime? LastBackupDate { get; set; }
    }
}
