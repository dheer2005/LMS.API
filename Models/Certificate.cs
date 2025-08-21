namespace LMS.API.Models
{
    public class Certificate
    {
        public int CertificateId { get; set; }
        public Guid CertificateGuid { get; set; } = Guid.NewGuid();
        public string CertificateNumber { get; set; }   
        public DateTime IssuedOn { get; set; } = DateTime.Now;
        public int UserId { get; set; }
        public User User { get; set; }
        public int CourseId { get; set; }
        public Course Course { get; set; }
    }
}
