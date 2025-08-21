namespace LMS.API.Models
{
    public class Quiz
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int CourseId { get; set; }
        public Course Course { get; set; }
        public List<Question> Questions { get; set; }
    }
}
