using LMS.API.DTOs;
using LMS.API.Helpers;
using LMS.API.LMSDbContext;
using LMS.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VideoController : Controller
    {
        private readonly LMSContext _context;
        private readonly CloudinaryService _cloudinaryService;
        public VideoController(CloudinaryService cloudinaryService, LMSContext context)
        {
            _context = context;
            _cloudinaryService = cloudinaryService;
        }

        [HttpPost("upload")] 
        [RequestSizeLimit(long.MaxValue)]
        public async Task<IActionResult> UploadVideo([FromForm] VideoUploadDto dto)
        {
            var setting = await _context.SystemSettings.FirstOrDefaultAsync(x => x.Id == 1);

            if (dto.File == null || dto.File.Length == 0)
                return BadRequest("File missing");

            double fileSizeInMB = dto.File.Length / (1024.0 * 1024.0);
            if (fileSizeInMB > setting?.MaxVideoUploadSizeMB)
                return BadRequest(new { message = $"Video size must be less than {setting.MaxVideoUploadSizeMB}MB" });


            LMS.API.DTOs.VideoUploadResults result;
            try
            {
                result = await _cloudinaryService.UploadVideoAsync(dto.File);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message); 
            }

            var video = new Video
            {
                Title = dto.Title,
                FilePath = result.Url, 
                ThumbnailPath = result.Url.Replace(".mp4", ".jpg"),
                CourseId = dto.CourseId,
                Description = dto.Description,
                Quality480 = result.QualityUrls["480p"],
                Quality720 = result.QualityUrls["720p"],
                Quality1080 = result.QualityUrls["1080p"]
            };

            _context.Videos.Add(video);
            await _context.SaveChangesAsync();

            return Ok(video);
        }

        [HttpGet("by-course/{courseId}")]
        public IActionResult GetVideosByCourse(int courseId)
        {
            var videos = _context.Videos
                .Where(v => v.CourseId == courseId)
                .Select(v => new
                {
                    v.Id,
                    v.Title,
                    v.FilePath,
                    v.ThumbnailPath,
                    v.CourseId,
                    v.Description,
                    Quality480 = v.Quality480,
                    Quality720 = v.Quality720,
                    Quality1080 = v.Quality1080
                })
                .ToList();

            return Ok(videos);
        }

        [HttpGet("overview/{courseId}")]
        public IActionResult GetCourseOverview(int courseId)
        {
            var course = _context.Courses
                .Where(c => c.Id == courseId)
                .Select(c => new
                {
                    c.Id,
                    c.Title,
                    CourseDescription = c.Description,
                    c.ThumbnailPath,
                    Videos = c.Videos.Select(v => new
                    {
                        v.Id,
                        v.Title,
                        v.Description,
                        VideoUrls = new
                        {
                            _1080p = v.Quality1080,
                            _720p = v.Quality720,
                            _480p = v.Quality480
                        },
                        v.ThumbnailPath
                    }).ToList()
                })
                .FirstOrDefault();

            if (course == null)
                return NotFound();

            return Ok(course);
        }

        [HttpPost("track")]
        public async Task<IActionResult> TrackWatch([FromBody] VideoWatchHistoryDto model)
        {
            if(model.VideoId == 0 || model.StudentId == 0 || model.CourseId == 0)
                return BadRequest(new { message = "Invalid data" });

            var exists = await _context.VideoWatchHistories.AnyAsync(x => x.StudentId == model.StudentId && x.VideoId == model.VideoId);

            if (!exists)
            {
                var videoHistory = new VideoWatchHistory
                {
                    StudentId = model.StudentId,
                    CourseId = model.CourseId,
                    VideoId = model.VideoId,
                    WatchedAt = DateTime.Now,
                };

                _context.VideoWatchHistories.Add(videoHistory);
                await _context.SaveChangesAsync();  
            }

            return Ok(new { message = "Watch recorded" });
        }
    }
}
