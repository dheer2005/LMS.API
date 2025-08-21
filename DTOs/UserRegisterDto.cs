namespace LMS.API.DTOs
{
    public class UserRegisterDto
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public bool IsEmailVerified { get; set; }
        public string Password { get; set; }
        public IFormFile Signature { get; set; }
        public string Role { get; set; }
    }
}
