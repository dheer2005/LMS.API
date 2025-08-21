using System.Diagnostics;

namespace LMS.API.Helpers
{
    public class VideoProcessor
    {
        public static void GenerateThumbnail(string videoPath, string thumbnailPath)
        {
            var ffmpeg = "ffmpeg";
            var args = $"-i \"{videoPath}\" -ss 00:00:01.000 -vframes 1 \"{thumbnailPath}\"";

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = ffmpeg,
                    Arguments = args,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            process.Start();
            process.WaitForExit();
        }
    }
}
