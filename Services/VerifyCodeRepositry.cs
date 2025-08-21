using LMS.API.Interfaces;
using LMS.API.LMSDbContext;
using LMS.API.Models;

namespace LMS.API.Services
{
    public class VerifyCodeRepositry : IVerifyCodeRepository
    {
        private readonly LMSContext _context;
        public VerifyCodeRepositry(LMSContext context)
        {
            _context = context;
        }

        public async Task<bool> Upsert(VerifyCode code)
        {
            if (code.UserName == null || code.Code == null || code.Code?.Length <= 5)
            {
                return false;
            }

            var user = _context.VerifyCodes.FirstOrDefault(user => code.UserName == user.UserName);

            if (user != null) 
            {
                //already exist update
                user.Code = code.Code;
                var expire = DateTime.Now.AddMinutes(5);
                user.ExpireAt = expire;
            }
            else
            {
                //create new
                code.ExpireAt = DateTime.Now.AddMinutes(5);
                _context.VerifyCodes.Add(code);
            }

            //save changes
            await _context.SaveChangesAsync();
            return true;
        }

        public bool VerifyUser(string user, string code) 
        {
            if (user == null || code == null) 
                return false;

            var person = _context.VerifyCodes.FirstOrDefault(x => x.UserName == user);
            if (person == null || person.ExpireAt <= DateTime.Now || person.Code != code) 
            {
                return false;
            }

            return true;
        }
        
    }
}
