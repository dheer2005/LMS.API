namespace LMS.API.Models
{
    public class QuizSubmission
    {
        public int Id { get; set; } 
        public int UserId { get; set; } 
        public User User { get; set; }
        public int QuizId { get; set; }
        public Quiz Quiz { get; set; }
        public int AttemptNumber { get; set; }
        public DateTime SubmittedAt { get; set; } = DateTime.Now;
        public int Score { get; set; }  
    }
}
