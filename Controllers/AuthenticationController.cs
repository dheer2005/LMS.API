using LMS.API.DTOs;
using LMS.API.Helpers;
using LMS.API.Interfaces;
using LMS.API.LMSDbContext;
using LMS.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly LMSContext _context;
        private readonly IConfiguration _config;
        private readonly IEmailService _emailService;
        private readonly IVerifyCodeRepository _verifyCodeRepository;
        private readonly CloudinaryService _cloudinaryService;

        public AuthController(LMSContext context, IConfiguration config, IVerifyCodeRepository verifyCodeRepository, IEmailService emailService, CloudinaryService cloudinaryService)
        {
            _context = context;
            _config = config;
            _verifyCodeRepository = verifyCodeRepository;
            _emailService = emailService;
            _cloudinaryService = cloudinaryService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] UserRegisterDto dto)
        {
            var setting =await _context.SystemSettings.FirstOrDefaultAsync(x=>x.Id == 1);

            if(setting != null)
            {
                if (setting.RegistrationEnabled == false)
                    return BadRequest(new { message = "Admin disabled public registration" });

                if (dto.Password.Length < setting.MinPasswordLength)
                    return BadRequest(new { message = $"Password must be gerater than or equals to {setting.MinPasswordLength}" });

                if (_context.Users.Any(u => u.Email == dto.Email))
                    return BadRequest(new { message = "Email already exists" });
            }

            string SignatureUrl = null;

            if (dto.Signature != null)
            {
                SignatureUrl = await _cloudinaryService.UploadImageAsync(dto.Signature);
            }

            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                IsEmailVerified = dto.IsEmailVerified,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Signature = SignatureUrl ?? " ",
                Role = dto.Role
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return Ok(new { message = "User Registered" });
        }

        [HttpPost("AlreadyExists")]
        public async Task<IActionResult> CheckExists([FromBody] UserRegisterDto dto)
        {
            var emailExists = await _context.Users.FirstOrDefaultAsync(x => x.Email == dto.Email);
            if (emailExists != null)
            {
                return BadRequest(new { message = "Email alreay exists" });
            }
            return Ok();
        } 

        [HttpPost("login")]
        public IActionResult Login([FromBody] UserLoginDto dto)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == dto.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return Unauthorized("Invalid credentials");

            var setting = _context.SystemSettings.FirstOrDefault(x => x.Id == 1);
            int sessionTimeoutMin = setting?.SessionTimeoutMinutes ?? 60;

            var key = Encoding.UTF8.GetBytes(_config["JWT:Secret"]!);

            //var now = DateTime.UtcNow;

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim("fullname", user.FullName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
                }),

                NotBefore = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddMinutes(sessionTimeoutMin),

                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _config["JWT:Issuer"],
                Audience = _config["JWT:Audience"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwt = tokenHandler.WriteToken(token);

            return Ok(new { token = jwt, user.Role });
        }

        [HttpPost("SendEmail")]
        public async Task<IActionResult> SendEmail(EmailInfo email)
        {
            try
            {
                Random rnd = new Random();
                string sixDigit = rnd.Next(0, 1000000).ToString("D6");
                var code = new VerifyCode
                {
                    UserName = email.UserName,
                    Code = sixDigit,
                };

                await _verifyCodeRepository.Upsert(code);

                email.Subject = $"Verify your Email";
                email.Body = $@"<!DOCTYPE html>
                <html lang=""en"">
                <head>
                    <meta charset=""UTF-8"">
                    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                    <title>LMS Verification Code</title>
                    <style>
                        body {{
                            font-family: Arial, sans-serif;
                            background-color: #f0f2f5;
                            margin: 0;
                            padding: 0;
                        }}
                        .container {{
                            max-width: 600px;
                            margin: 20px auto;
                            background: linear-gradient(135deg, #4A90E2, #0056b3);
                            padding: 25px;
                            border-radius: 10px;
                            box-shadow: 0 4px 15px rgba(0,0,0,0.2);
                            color: white;
                            text-align: center;
                        }}
                        .logo {{
                            font-size: 40px;
                            margin-bottom: 15px;
                        }}
                        h1 {{
                            margin: 10px 0;
                            font-size: 26px;
                        }}
                        p {{
                            font-size: 16px;
                            color: #e0e0e0;
                            margin-bottom: 15px;
                        }}
                        .code {{
                            display: inline-block;
                            font-size: 28px;
                            font-weight: bold;
                            color: #0056b3;
                            background-color: #fff;
                            padding: 12px 25px;
                            border-radius: 6px;
                            margin-top: 15px;
                            letter-spacing: 4px;
                        }}
                        .note {{
                            margin-top: 20px;
                            font-size: 14px;
                            color: #dcdcdc;
                        }}
                        @media (max-width: 600px) {{
                            .container {{
                                padding: 15px;
                            }}
                            h1 {{
                                font-size: 22px;
                            }}
                            .code {{
                                font-size: 24px;
                                padding: 10px 20px;
                            }}
                        }}
                    </style>
                </head>
                <body>
                    <div class=""container"">
                        <div class=""logo"">🎓</div>
                        <h1>LMS Verification Code</h1>
                        <p>Thank you for signing up for our Learning Management System. Your verification code is:</p>
                        <div class=""code"">{sixDigit}</div>
                        <p class=""note"">Please use this code within 5 minutes to complete your registration.</p>
                    </div>
                </body>
                </html>";
                await _emailService.SendEmailAsync(email);
                return Ok();
            }
            catch (Exception ex) 
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("verify/{userName}/{userCode}")]
        public IActionResult VerifyCode(string userName, string userCode) 
        {
            var verified = _verifyCodeRepository.VerifyUser(userName, userCode);
            if (verified)
            {
                return Ok();
            }

            return BadRequest(new { message = "Otp invalid" });
        }

    }

}
