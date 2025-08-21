namespace LMS.API.DTOs
{
    public class RaiseDoubtDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public int CourseId { get; set; }
    }
}
