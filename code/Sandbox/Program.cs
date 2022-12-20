
using System.Net.Http.Json;

namespace Sandbox
{
    public class SandBoxClass
    {
        static async Task Main(string[] args)
        {
            await UploadImages();
        }
        
        static async Task UploadImages()
        {
            var imgDir = "imageDone";

            var images = Directory.GetFiles(imgDir);

            HttpClient client = new HttpClient();

            foreach (var image in images)
            {
                var fileInfo = new FileInfo(image);
                var imageBytes = File.ReadAllBytes(image);
                string imageData = Convert.ToBase64String(imageBytes);

                var dto = new { iconName = fileInfo.Name, iconData = imageData };

                await client.PostAsJsonAsync("https://localhost:7229/api/v1/Icons", dto);

                Console.WriteLine("Posted " + fileInfo.Name);
            }
        }
    }
}

