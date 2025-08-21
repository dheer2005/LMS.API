namespace LMS.API.Models
{
    public class Video
    {
        public int Id { get; set; }

        public string Title { get; set; }
        public string FilePath { get; set; }
        public string Quality480 { get; set; }
        public string Quality720 { get; set; }
        public string Quality1080 { get; set; }
        public string Description { get; set; }
        public string ThumbnailPath { get; set; }
        public int CourseId { get; set; }
        public Course Course { get; set; }
    }
}
