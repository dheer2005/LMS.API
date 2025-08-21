using LMS.API.Models;
using Microsoft.EntityFrameworkCore;

namespace LMS.API.LMSDbContext
{
    public class LMSContext : DbContext
    {
        public LMSContext(DbContextOptions<LMSContext> options): base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Course → Video (Cascade delete)
            modelBuilder.Entity<Video>()
                .HasOne(v => v.Course)
                .WithMany(c => c.Videos)
                .HasForeignKey(v => v.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            // Course → Enrollment (Cascade delete)
            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Course)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            // Enrollment → Student (Restrict to avoid cycles)
            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Student)
                .WithMany()
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Course → Teacher (Restrict to avoid cycles)
            modelBuilder.Entity<Course>()
                .HasOne(c => c.Teacher)
                .WithMany()
                .HasForeignKey(c => c.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);

            // Quiz → Question (Cascade delete)
            modelBuilder.Entity<Question>()
                .HasOne(q => q.Quiz)
                .WithMany(z => z.Questions)
                .HasForeignKey(q => q.QuizId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<QuizSubmission>()
                .HasOne(qs => qs.User)
                .WithMany()
                .HasForeignKey(qs => qs.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // QuizSubmission → Quiz (Many-to-One)
            modelBuilder.Entity<QuizSubmission>()
                .HasOne(qs => qs.Quiz)
                .WithMany()
                .HasForeignKey(qs => qs.QuizId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SystemSetting>().HasData(new SystemSetting
            {
                Id = 1,
                PlatformName = "Smart LMS",
                MaxVideoUploadSizeMB = 500,
                AllowedVideoFormats = "mp4,webm,mov",
                QuizRetakeLimit = 2,
                RegistrationEnabled = true,
                RequireEmailVerification = false,
                MinPasswordLength = 9,
                SessionTimeoutMinutes = 30,
                PaidCoursesEnabled = false,
                LogoUrl = "https://www.bing.com/images/search?q=logo.jpg&FORM=IQFRBA&id=5CE7D99227F3B6773B0026254E307A3A89034874"
            });

            // VideoWatchHistory → Student
            modelBuilder.Entity<VideoWatchHistory>()
                .HasOne(v => v.Student)
                .WithMany()
                .HasForeignKey(v => v.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            // VideoWatchHistory → Course
            modelBuilder.Entity<VideoWatchHistory>()
                .HasOne(v => v.Course)
                .WithMany()
                .HasForeignKey(v => v.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            // VideoWatchHistory → Video
            modelBuilder.Entity<VideoWatchHistory>()
                .HasOne(v => v.Video)
                .WithMany()
                .HasForeignKey(v => v.VideoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Doubt>()
                .HasOne(d => d.Course)
                .WithMany(c => c.Doubts)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.Restrict); 

            modelBuilder.Entity<Doubt>()
                .HasOne(d => d.Student)
                .WithMany()
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.Restrict); 

            modelBuilder.Entity<DoubtReply>()
                .HasOne(r => r.Doubt)
                .WithMany(d => d.Replies)
                .HasForeignKey(r => r.DoubtId)
                .OnDelete(DeleteBehavior.Cascade); 

            modelBuilder.Entity<DoubtReply>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Certificate>(e =>
            {
                e.HasIndex(x => x.CertificateGuid).IsUnique();
                e.HasIndex(x => x.CertificateNumber).IsUnique();

                e.HasOne(x => x.User)
                 .WithMany()
                 .HasForeignKey(x => x.UserId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.Course)
                 .WithMany()
                 .HasForeignKey(x => x.CourseId)
                 .OnDelete(DeleteBehavior.Restrict);
            });
        }

        public DbSet<User> Users { get; set; }  
        public DbSet<Course> Courses { get; set; }
        public DbSet<Video> Videos {  get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<SystemSetting> SystemSettings { get; set; }
        public DbSet<QuizSubmission> QuizSubmissions { get; set; }
        public DbSet<VideoWatchHistory> VideoWatchHistories { get; set; }
        public DbSet<VerifyCode> VerifyCodes { get; set; }
        public DbSet<Doubt> Doubts { get; set; }
        public DbSet<DoubtReply> DoubtReplies { get; set; }
        public DbSet<Certificate> Certificates { get; set; }
    }
}
