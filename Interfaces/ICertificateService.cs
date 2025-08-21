using LMS.API.Models;

namespace LMS.API.Interfaces
{
    public interface ICertificateService
    {
        Task<Certificate> GenerateAsync(int userId, int courseId);
        Task<Certificate> GetByGuidAsync(Guid guid);
        Task<byte[]> GeneratePdfAsync(int certificateId);
    }
}
