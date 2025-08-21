using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace LMS.API.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ThumbnailPath { get; set; }
        public int TeacherId { get; set; }
        public User Teacher { get; set; }
        public decimal Price { get; set; }
        public ICollection<Video> Videos { get; set; }
        public ICollection<Enrollment> Enrollments { get; set; }
        public ICollection<Doubt> Doubts { get; set; }
    }
}
