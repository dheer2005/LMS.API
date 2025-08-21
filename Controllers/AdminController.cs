using LMS.API.DTOs;
using LMS.API.Helpers;
using LMS.API.LMSDbContext;
using LMS.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : Controller
    {
        private readonly CloudinaryService _cloudinary;
        private readonly LMSContext _context;
        public AdminController(LMSContext context, CloudinaryService cloudinary)
        {
            _context = context;
            _cloudinary = cloudinary;
        }
        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _context.Users.Where(user=>user.Role != "Admin").ToListAsync();
            return Ok(users);
        }

        [HttpDelete("deleteUser/{userId}")]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (user == null)
                BadRequest(new { message = "User not found" });

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(user);
        }

        [HttpGet("getPasswordLength")]
        public async Task<IActionResult> GetPassLength()
        {
            var length = await _context.SystemSettings.FirstAsync();
            return Ok(length.MinPasswordLength);
        }

        [HttpPut("updateUser/{userId}")]
        public async Task<IActionResult> UpdateUser(int userId,[FromForm] UserUpdateDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                return BadRequest(new { message = "User not found" });

            var emailExists = await _context.Users.AnyAsync(c => c.Email == dto.Email && c.Id != userId);
            if (emailExists)
            {
                return BadRequest(new { message = "email already exists" });
            }

            user.FullName = dto.FullName;
            user.Email = dto.Email;

            if(dto.Signature != null)
            {
                user.Signature = await _cloudinary.UploadImageAsync(dto.Signature);
            }

            await _context.SaveChangesAsync();
            return Ok(user);
        }


        [HttpGet("get-systemSetting")]
        public async Task<ActionResult<SystemSetting>> GetSetting()
        {
            var setting = await _context.SystemSettings.FirstOrDefaultAsync();
            if (setting == null) return NotFound();
            return Ok(setting);
        }

        [HttpPut("update-systemSetting/{settingId}")]
        public async Task<IActionResult> UpdateSetting(int settingId, [FromBody] SystemSetting updated)
        {
            if (settingId != updated.Id) return BadRequest();

            var existing = await _context.SystemSettings.FindAsync(settingId);
            if (existing == null) return NotFound();

            _context.Entry(existing).CurrentValues.SetValues(updated);
            await _context.SaveChangesAsync();

            return Ok(existing);
        }

        [HttpPost("upload-logo")]
        public async Task<IActionResult> UploadLogo([FromForm] IFormFileDto model)
        {
            if (model.file == null || model.file.Length == 0)
                return BadRequest("No file supplied");

            var url = await _cloudinary.UploadImageAsync(model.file);
            return Ok(new {url = url});
        }
    }


}
