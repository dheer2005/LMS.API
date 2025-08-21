using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using QuestPDF.Helpers;
using QRCoder;
using System.Drawing;
using System.IO;
using LMS.API.Interfaces;
using LMS.API.LMSDbContext;
using LMS.API.Models;

namespace LMS.API.Services
{
    public class CertificateService : ICertificateService
    {
        private readonly LMSContext _context;
        public CertificateService(LMSContext context)
        {
            _context = context;
        }
        public async Task<string> GenerateCertificateNumberAsync()
        {
            int latestId = await _context.Certificates.CountAsync()+1;
            string year = DateTime.Now.Year.ToString();
            return $"CERT-{year}-{latestId:D4}";
        }


        public async Task<Certificate> GenerateAsync(int userId, int courseId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId)
                       ?? throw new ArgumentException("User not found");

            var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == courseId)
                         ?? throw new ArgumentException("Course not found");

            bool exists = await _context.Certificates.AnyAsync(c => c.UserId == userId && c.CourseId == courseId);
            if (exists) throw new InvalidOperationException("Certificate already issued for this user & course.");

            var certificate = new Certificate
            {
                UserId = userId,
                CourseId = courseId,
                CertificateNumber = await GenerateCertificateNumberAsync(),
                IssuedOn = DateTime.UtcNow
            };

            _context.Certificates.Add(certificate);
            await _context.SaveChangesAsync();

            // Load navigation for responses / PDF
            await _context.Entry(certificate).Reference(c => c.User).LoadAsync();
            await _context.Entry(certificate).Reference(c => c.Course).LoadAsync();

            return certificate;
        }

        public async Task<Certificate?> GetByGuidAsync(Guid guid)
        {
            return await _context.Certificates
                .Include(c => c.User)
                .Include(c => c.Course)
                .FirstOrDefaultAsync(c => c.CertificateGuid == guid);
        }


        public async Task<byte[]> GeneratePdfAsync(int certificateId)
        {
            try
            {
                var cert = await _context.Certificates
                    .Include(c => c.User)
                    .Include(c => c.Course)
                        .ThenInclude(c => c.Teacher)
                    .FirstOrDefaultAsync(c => c.CertificateId == certificateId);

                if (cert == null)
                    throw new ArgumentException("Certificate not found");

                string verificationUrl = $"https://lms-client-bice.vercel.app/verify-certificate/{cert.CertificateGuid}";

                using var qrGenerator = new QRCodeGenerator();
                using var qrData = qrGenerator.CreateQrCode(verificationUrl, QRCodeGenerator.ECCLevel.Q);
                using var qrCode = new PngByteQRCode(qrData);
                byte[] qrImageBytes = qrCode.GetGraphic(15);
                using var httpClient = new HttpClient();
                byte[] signatureBytes = await httpClient.GetByteArrayAsync((cert.Course.Teacher.Signature).Trim('"'));

                byte[] sealImageBytes = await File.ReadAllBytesAsync("wwwroot/images/SmartLms-stamp.png");

                var doc = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4.Landscape());
                        page.Margin(2, Unit.Centimetre);
                        page.DefaultTextStyle(x => x.FontSize(14));
                        page.Background().Border(4).BorderColor("#1e3a8a");

                        page.Header().Row(row =>
                        {
                            row.RelativeItem().AlignLeft()
                                .Text($"GUID: {cert.CertificateGuid}")
                                .FontSize(10).FontColor("#374151");

                            row.ConstantItem(80).AlignRight().Image(qrImageBytes);
                        });

                        page.Content().Column(col =>
                        {
                            col.Item().AlignCenter().Text("Certificate of Completion")
                                .FontSize(38).Bold().FontColor("#1e3a8a");

                            col.Item().PaddingTop(20).AlignCenter().Text("This certifies that")
                                .FontSize(18).Italic();

                            col.Item().AlignCenter().Text(cert.User.FullName ?? "")
                                .FontSize(30).Bold().FontColor("#111827");

                            col.Item().PaddingTop(10).AlignCenter().Text("has successfully completed the course")
                                .FontSize(18);

                            col.Item().AlignCenter().Text(cert.Course.Title ?? "")
                                .FontSize(28).Bold().FontColor("#047857");

                            col.Item().PaddingTop(25).AlignCenter().Row(row =>
                            {
                                row.RelativeItem().AlignCenter()
                                    .Text($"Issued On: {cert.IssuedOn:dd MMM yyyy}")
                                    .FontSize(14).FontColor("#374151");

                                row.RelativeItem().AlignCenter()
                                    .Text($"Certificate No: {cert.CertificateNumber}")
                                    .FontSize(14).FontColor("#374151");
                            });

                            col.Item().PaddingTop(50).Row(row =>
                            {
                                row.RelativeItem().AlignCenter().Column(sig =>
                                {
                                    sig.Item().AlignCenter().Container().Height(60).Width(60).Image(sealImageBytes).FitArea();

                                    sig.Item().PaddingTop(10).AlignCenter().Text("Smart LMS Stamp").FontSize(12).Italic();
                                });

                                row.RelativeItem().PaddingTop(10).AlignCenter().Column(sig =>
                                {
                                    if (signatureBytes == null)
                                    {
                                        sig.Item().AlignCenter().Text(cert.Course.Teacher?.FullName ?? "").FontSize(12).Italic();
                                    }
                                    else
                                    {
                                        sig.Item().AlignCenter().Container().Height(40).Width(100).Image(signatureBytes).FitArea();
                                    }

                                    sig.Item().PaddingTop(10).AlignCenter().Text("Authorized Signatory").FontSize(12).Italic();
                                });
                            });
                        });

                        page.Footer()
                            .AlignCenter()
                            .Text("© Your LMS Platform – This certificate is digitally generated and verifiable online")
                            .FontSize(9).FontColor("#9ca3af");
                    });
                });

                return doc.GeneratePdf();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"PDF generation failed: {ex.Message}", ex);
            }
        }

    }
}
