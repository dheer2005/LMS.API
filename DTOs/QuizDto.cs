namespace LMS.API.DTOs
{
    public class QuizDto
    {
        public int CourseId { get; set; }
        public string Title { get; set; }
        public int AttemptNumber { get; set; }
        public List<QuestionDto> Questions { get; set; }
    }
}
