namespace LMS.API.Models
{
    public class VideoWatchHistory
    {
        public int Id { get; set; }              
        public int StudentId { get; set; }      
        public int CourseId { get; set; }        
        public int VideoId { get; set; }        
        public DateTime WatchedAt { get; set; }  
        public User Student { get; set; }
        public Course Course { get; set; }
        public Video Video { get; set; }
    }
}
