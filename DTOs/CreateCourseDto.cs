namespace LMS.API.DTOs
{
    public class CreateCourseDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int TeacherId { get; set; }
        public IFormFile Thumbnail { get; set; }
        public decimal Price { get; set; }
    }
}
