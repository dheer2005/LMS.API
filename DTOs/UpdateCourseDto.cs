namespace LMS.API.DTOs
{
    public class UpdateCourseDto
    {
        public string Title { get; set; }
        public string Description { get; set; } 
        public IFormFile Thumbnail { get; set; }
        public decimal Price { get; set; }
    }
}
