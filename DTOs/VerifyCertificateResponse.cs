namespace LMS.API.DTOs
{
    public class VerifyCertificateResponse
    {
        public string CertificateNumber { get; set; } = string.Empty;
        public DateTime IssuedOn { get; set; }
        public string User { get; set; } = string.Empty;
        public string Course { get; set; } = string.Empty;
    }
}
