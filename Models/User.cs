namespace LMS.API.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public bool IsEmailVerified { get; set; } = false;  
        public string PasswordHash { get; set; }
        public string Signature { get; set; }
        public string Role { get; set; }
    }
}
