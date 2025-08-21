namespace LMS.API.DTOs
{
    public class VideoUploadResults
    {
        public string PublicId { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public Dictionary<string, string> QualityUrls { get; set; } = new();
    }
}
