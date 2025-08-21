using LMS.API.LMSDbContext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LMS.API.Controllers
{
    [ApiController]
    [Route("api/{controller}")]
    public class StudentProgressController : Controller
    {
        private readonly LMSContext _context;
        public StudentProgressController(LMSContext context)
        {
            _context = context;
        }

        [HttpGet("progress/{studentId}")]
        public async Task<IActionResult> GetStudentProgress(int studentId)
        {
            var student = await _context.Users.FirstOrDefaultAsync(u=>u.Id == studentId && u.Role == "Student");
            if (student == null)
                return NotFound("Student not found");

            var enrolledCourses = await _context.Enrollments
                .Where(e=>e.StudentId == studentId)
                .Include(c=>c.Course)
                .ToListAsync();

            var progressList = enrolledCourses.Select(e => new
            {
                CourseId = e.CourseId,
                CourseTitle = e.Course.Title,
                TotalVideos = _context.Videos.Count(v => v.CourseId == e.CourseId),
                WatchedVideos = _context.VideoWatchHistories.Count(v => v.CourseId == e.CourseId && v.StudentId == studentId),
                QuizzesAttempted = _context.QuizSubmissions.Count(q => q.UserId == studentId && q.Quiz.CourseId == e.CourseId),
                CompletionPercentage = CalculateCompletionPercentage(studentId, e.CourseId)
            });

            return Ok(progressList);
        }

        private int CalculateCompletionPercentage(int studentId, int courseId)
        {
            var totalVideos = _context.Videos.Count(v => v.CourseId == courseId);
            var watched = _context.VideoWatchHistories.Count(w => w.CourseId == courseId && w.StudentId == studentId);
            if (totalVideos == 0) return 0;
            return (int)((watched / (float)totalVideos) * 100);
        }
    }
}
