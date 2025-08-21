using LMS.API.DTOs;
using LMS.API.Helpers;
using LMS.API.LMSDbContext;
using LMS.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CourseController : Controller
    {
        private readonly LMSContext _context;
        private readonly CloudinaryService _cloudinaryService;
        public CourseController(LMSContext context, CloudinaryService cloudinaryService )
        {
            _context = context;
            _cloudinaryService = cloudinaryService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateCourse([FromForm] CreateCourseDto model)
        {
            string thumbnailUrl = null;

            if (model.Thumbnail != null)
            {
                thumbnailUrl = await _cloudinaryService.UploadImageAsync(model.Thumbnail);
            }

            var course = new Course
            {
                Title = model.Title,
                Description = model.Description,
                ThumbnailPath = thumbnailUrl,
                TeacherId = model.TeacherId,
                Price = model.Price
            };

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Course created", course });
        }

        [HttpGet("my-courses/{teacherId}")]
        public IActionResult GetMyCourses(int teacherId)
        {
            var courses = _context.Courses
                .Where(c => c.TeacherId == teacherId)
                .Select(c => new
                {
                    c.Id,
                    c.Title,
                    c.Description,
                    c.ThumbnailPath,
                    c.Price
                })
                .ToList();

            return Ok(courses);
        }

        [HttpGet("available-courses/{studentId}")]
        public IActionResult GetAvailableCourses(int studentId)
        {
            var courses = _context.Courses
                .Select(c => new
                {
                    c.Id,
                    c.Title,
                    c.Description,
                    c.ThumbnailPath,
                    c.Price,
                    IsEnrolled = _context.Enrollments.Any(e => e.CourseId == c.Id && e.StudentId == studentId)
                })
                .ToList();

            return Ok(courses);
        }

        [HttpGet("isEnrolled/{studentId}/{courseId}")]
        public async Task<IActionResult> IsEnrolled(int studentId, int courseId)
        {
            var isEnrolled = await _context.Enrollments.AnyAsync(x => x.CourseId == courseId && x.StudentId == studentId);
            return Ok(isEnrolled);
        }

        [HttpDelete("deleteCourse/{courseId}")]
        public async Task<IActionResult> DeleteCourse(int courseId)
        {
            var course = await _context.Courses.FirstOrDefaultAsync(x=>x.Id == courseId);
            if (course == null)
                BadRequest("Course not found");

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
            return Ok(new {message = "Course deleted successfully"});
        }

        [HttpPut("updateCourseDetails/{courseId}")]
        public async Task<IActionResult> UpdateCourse(int courseId, UpdateCourseDto dto)
        {
            var course = _context.Courses.FirstOrDefault(x=>x.Id == courseId);
            if (course == null)
                BadRequest(new { message = "Course not found" });

            var thumbnailUrl = "";
            if(dto.Thumbnail != null)
            {
                thumbnailUrl = await _cloudinaryService.UploadImageAsync(dto.Thumbnail);
            }

            course.Title = dto.Title;   
            course.Description = dto.Description;
            course.ThumbnailPath = thumbnailUrl;
            course.Price = dto.Price;

            await _context.SaveChangesAsync();

            return Ok(course);
        }

        [HttpPost("enroll")]
        public IActionResult EnrollInCourse([FromBody] EnrollRequestDto model)
        {
            var alreadyEnrolled = _context.Enrollments.Any(e =>
                e.CourseId == model.CourseId && e.StudentId == model.StudentId);

            if (alreadyEnrolled)
                return BadRequest("Already enrolled in this course.");

            var enrollment = new Enrollment
            {
                CourseId = model.CourseId,
                StudentId = model.StudentId
            };

            _context.Enrollments.Add(enrollment);
            _context.SaveChanges();

            return Ok(new { message = "Enrollment successful." });
        }

    }
}
