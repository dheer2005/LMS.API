namespace LMS.API.Models
{
    public class VerifyCode
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Code { get; set; }

        public DateTime ExpireAt { get; set; }
    }
}
