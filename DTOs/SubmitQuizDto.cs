namespace LMS.API.DTOs
{
    public class SubmitQuizDto
    {
        public int QuizId { get; set; }

        public List<AnswerSubmissionDto> Answers { get; set; }
    }
}
