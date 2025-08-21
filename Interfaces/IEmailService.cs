using LMS.API.DTOs;

namespace LMS.API.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(EmailInfo email);
    }
}
