using LMS.API.Models;

namespace LMS.API.Interfaces
{
    public interface IVerifyCodeRepository
    {
        Task<bool> Upsert(VerifyCode code);
        bool VerifyUser(string user, string code);
    }
}
