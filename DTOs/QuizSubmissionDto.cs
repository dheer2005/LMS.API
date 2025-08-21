namespace LMS.API.DTOs
{
    public class QuizSubmissionDto
    {
        public int UserId { get; set; } 
        public int QuizId { get; set; }
        public int Score { get; set; }  
        public int AttemptNumber { get; set; }

    }
}
