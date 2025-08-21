namespace LMS.API.Models
{
    public class Doubt
    {
        public int Id { get; set; }

        public int CourseId { get; set; }
        public Course Course { get; set; }

        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public User Student { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }

        public DateTime CreatedAt { get; set; }
        public bool IsResolved { get; set; }

        public ICollection<DoubtReply> Replies { get; set; }
    }
}
