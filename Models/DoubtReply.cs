namespace LMS.API.Models
{
    public class DoubtReply
    {
        public int Id { get; set; }

        public int DoubtId { get; set; }
        public Doubt Doubt { get; set; }

        public int TeacherId { get; set; }
        public string TeacherName { get; set; }
        public User User { get; set; }

        public string ReplyText { get; set; }
        public string AttachmentUrl { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
