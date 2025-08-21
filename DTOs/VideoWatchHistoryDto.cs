namespace LMS.API.DTOs
{
    public class VideoWatchHistoryDto
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public int VideoId { get; set; }
    }
}
