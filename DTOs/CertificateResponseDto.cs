namespace LMS.API.DTOs
{
    public class CertificateResponseDto
    {
        public int CertificateId { get; set; }
        public Guid CertificateGuid { get; set; }
        public string CertificateNumber { get; set; }
        public DateTime IssuedOn { get; set; }
        public int UserId { get; set; }
        public string UserFullName { get; set; }
        public int CourseId { get; set; }   
        public string CourseTitle { get; set; }
    }
}
