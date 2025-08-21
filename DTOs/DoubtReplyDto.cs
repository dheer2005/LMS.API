namespace LMS.API.DTOs
{
    public class DoubtReplyDto
    {
        public int DoubtId { get; set; }
        public int TeacherId { get; set; }
        public string TeacherName { get; set; }
        public string ReplyText { get; set; }
        public string AttachmentUrl { get; set; }
    }
}
