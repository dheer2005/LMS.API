using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using LMS.API.DTOs;

namespace LMS.API.Helpers
{
    public class CloudinaryService
    {
        private readonly Cloudinary _cloudinary;
        public CloudinaryService(IConfiguration config)
        {
            var account = new Account(
                config["CloudinarySettings:CloudName"],
                config["CloudinarySettings:ApiKey"],
                config["CloudinarySettings:ApiSecret"]
            );
            _cloudinary = new Cloudinary(account);
        }

        public async Task<VideoUploadResults> UploadVideoAsync(IFormFile file)
        {
            await using var stream = file.OpenReadStream();

            var publicId = Path.GetFileNameWithoutExtension(file.FileName);

            var uploadParams = new VideoUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                PublicId = publicId,
                UseFilename = true,
                UniqueFilename = false,
                Overwrite = true
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult.StatusCode != System.Net.HttpStatusCode.OK)
                throw new Exception(uploadResult.Error?.Message ?? "Unknown upload error");
            string basePublicId = uploadResult.PublicId;

            return new VideoUploadResults
            {
                PublicId = basePublicId,
                Url = uploadResult.SecureUrl.ToString(),
                QualityUrls = new Dictionary<string, string>
                {
                    { "1080p", _cloudinary.Api.UrlVideoUp.Transform(new Transformation().Width(1920).Height(1080).Crop("scale")).BuildUrl($"{basePublicId}.mp4") },
                    { "720p",  _cloudinary.Api.UrlVideoUp.Transform(new Transformation().Width(1280).Height(720).Crop("scale")).BuildUrl($"{basePublicId}.mp4") },
                    { "480p",  _cloudinary.Api.UrlVideoUp.Transform(new Transformation().Width(854).Height(480).Crop("scale")).BuildUrl($"{basePublicId}.mp4") }
                }
            };
        }


        public async Task<string> UploadImageAsync(IFormFile file)
        {
            if (file.Length == 0)
                throw new ArgumentException("File is empty");

            await using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = "lms/thumbnails",
                UseFilename = true,
                UniqueFilename = false,
                Overwrite = true
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return uploadResult.SecureUrl.ToString();
            }
            else if (uploadResult.Error != null)
            {
                throw new Exception("Image upload failed: " + uploadResult.Error.Message);
            }

            throw new Exception("Image upload failed due to unknown error.");
        }
    }
}
