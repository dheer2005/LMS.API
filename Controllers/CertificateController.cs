using LMS.API.DTOs;
using LMS.API.Interfaces;
using LMS.API.LMSDbContext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CertificateController : Controller
    {
        private readonly ICertificateService _service;
        private readonly LMSContext _context;
        public CertificateController(LMSContext context, ICertificateService service)
        {
            _context = context;
            _service = service;
        }

        [HttpGet("get-certificate/{userId}/{courseId}")]
        public async Task<IActionResult> GetGeneratedCertiticate(int userId, int courseId)
        {
            var cert = await _context.Certificates.Include(u=>u.User).Include(c=>c.Course).FirstOrDefaultAsync(x => x.UserId == userId && x.CourseId == courseId);

            var newresponse = new CertificateResponseDto
            {
                CertificateId = cert.CertificateId,
                CertificateNumber = cert.CertificateNumber,
                CertificateGuid = cert.CertificateGuid,
                UserId = cert.UserId,
                UserFullName = cert.User.FullName,
                CourseId = cert.CourseId,
                CourseTitle = cert.Course.Title,
                IssuedOn = cert.IssuedOn
            };
            return Ok(newresponse);
        }


        [HttpPost("generate/{userId}/{courseId}")]
        public async Task<ActionResult<CertificateResponseDto>> Generate(int userId, int courseId)
        {
            try
            {
                var cert = await _service.GenerateAsync(userId, courseId);

                var dto = new CertificateResponseDto
                {
                    CertificateId = cert.CertificateId,
                    CertificateGuid = cert.CertificateGuid,
                    CertificateNumber = cert.CertificateNumber,
                    IssuedOn = cert.IssuedOn,
                    UserId = userId,
                    UserFullName = cert.User.FullName,
                    CourseId = cert.CourseId,
                    CourseTitle = cert.Course.Title
                };
                return Ok(dto);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("verify/{guid}")]
        public async Task<ActionResult<VerifyCertificateResponse>> Verify(Guid guid)
        {
            var cert = await _service.GetByGuidAsync(guid);
            if (cert == null) return NotFound("Certificate not found");

            var verifyresponse = new VerifyCertificateResponse
            {
                CertificateNumber = cert.CertificateNumber,
                IssuedOn = cert.IssuedOn,
                User = cert.User.FullName,
                Course = cert.Course.Title
            };

            return Ok(verifyresponse);
        }

        [HttpGet("download/{certificateId}")]
        public async Task<IActionResult> Download(int certificateId)
        {
            try
            {
                var pdfBytes = await _service.GeneratePdfAsync(certificateId);
                return File(pdfBytes, "application/pdf", $"Certificate_{certificateId}.pdf");
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch(Exception ex)
            {
                return StatusCode(500, new { message = "Error generating certificate", detail = ex.Message });
            }
        }
    }
}
