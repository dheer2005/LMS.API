namespace LMS.API.DTOs
{
    public class UserUpdateDto
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public IFormFile? Signature { get; set; }
    }
}
