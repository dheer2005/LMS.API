using Microsoft.AspNetCore.Mvc;

namespace LMS.API.DTOs
{
    public class VideoUploadDto
    {
        [FromForm(Name = "file")]
        public IFormFile File { get; set; }

        [FromForm(Name = "title")]
        public string Title { get; set; }

        [FromForm(Name = "courseId")]
        public int CourseId { get; set; }

        [FromForm(Name = "description")]
        public string Description { get; set; }

    }
}
