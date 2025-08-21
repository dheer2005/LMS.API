using LMS.API.DTOs;
using LMS.API.Hubs;
using LMS.API.LMSDbContext;
using LMS.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace LMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DoubtController : Controller
    {
        private readonly LMSContext _context;
        private readonly IHubContext<DoubtHub> _hubContext;
        public DoubtController(LMSContext context, IHubContext<DoubtHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        [HttpPost("raise")]
        public async Task<IActionResult> RaiseDoubt([FromBody] RaiseDoubtDto dto)
        {
            var doubt = new Doubt
            {
                Title = dto.Title,
                Description = dto.Description,
                StudentId = dto.StudentId,
                StudentName = dto.StudentName,
                CourseId = dto.CourseId,
                CreatedAt = DateTime.Now
            };
            _context.Doubts.Add(doubt);
            await _context.SaveChangesAsync();

            // Find teacher assigned to this course
            var teacher = await _context.Courses
                .Where(c => c.Id == doubt.CourseId)
                .Select(c => c.TeacherId) 
                .FirstOrDefaultAsync();

            if (teacher != 0)
            {
                await _hubContext.Clients.User(teacher.ToString()).SendAsync("ReceiveDoubt", new
                {
                    doubt.Id,
                    doubt.Title,
                    doubt.Description,
                    doubt.StudentId,
                    doubt.StudentName,
                    doubt.CourseId,
                    doubt.CreatedAt
                });
            }

            return Ok(new { message = "Doubt raised and teacher notified" });
        }

        [HttpGet("course/{courseId}")]
        public async Task<IActionResult> GetCourseDoubts(int courseId)
        {
            var doubts = await _context.Doubts
                .Where(d => d.CourseId == courseId)
                .Select(d => new
                {
                    d.Id,
                    d.Title,
                    d.Description,
                    d.IsResolved,
                    d.CreatedAt,
                    Student = new
                    {
                        d.Student.Id,
                        d.Student.FullName
                    },
                    Replies = d.Replies.Select(r => new {
                        r.Id,
                        r.ReplyText,
                        r.CreatedAt
                    })
                })
                .ToListAsync();

            return Ok(doubts);
        }

        [HttpGet("mine/{studentId}/{courseId}")]
        public async Task<IActionResult> GetStudentDoubts(int studentId, int courseId)
        {
            var doubts = await _context.Doubts
                    .Where(d => d.StudentId == studentId && d.CourseId == courseId)
                    .OrderByDescending(d => d.CreatedAt)
                    .Select(d => new
                    {
                        d.Id,
                        d.Title,
                        d.Description,
                        d.IsResolved,
                        d.CreatedAt,
                        Course = new
                        {
                            d.Course.Id,
                            d.Course.Title
                        },
                        Replies = d.Replies.Select(r => new
                        {
                            r.Id,
                            r.ReplyText,
                            r.CreatedAt,
                            Teacher = new
                            {
                                r.TeacherId,
                                r.User.FullName
                            }
                        })
                    })
                    .ToListAsync();

            return Ok(doubts);
        }

        [HttpPost("reply")]
        public async Task<IActionResult> ReplyToDoubt([FromBody] DoubtReplyDto dto)
        {
            var reply = new DoubtReply
            {
                DoubtId = dto.DoubtId,
                TeacherId = dto.TeacherId,
                TeacherName = dto.TeacherName,
                ReplyText = dto.ReplyText,
                AttachmentUrl = dto.AttachmentUrl,
                CreatedAt = DateTime.Now
            };
            _context.DoubtReplies.Add(reply);

            var doubt = await _context.Doubts.FindAsync(dto.DoubtId);
            if (doubt != null)
                doubt.IsResolved = true;

            await _context.SaveChangesAsync();

            await _hubContext.Clients.User(doubt.StudentId.ToString()).SendAsync("ReceiveDoubtReply", new
            {
                doubtId = dto.DoubtId,
                replyText = dto.ReplyText,
                teacherId = dto.TeacherId,
                teacherName = dto.TeacherName
            });

            return Ok(new { message = "Reply added and student notified" });
        }
    }
}
