using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;
namespace StackBook.Utils
{
    public class CloudinaryUtils
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryUtils(IOptions<CloudinarySettings> config)
        {
            var settings = config.Value;

            Account account = new Account(
                settings.CloudName,
                settings.ApiKey,
                settings.ApiSecret
            );

            _cloudinary = new Cloudinary(account);
        }

        public async Task<string> UploadImageAsync(IFormFile file)
        {
            using var stream = file.OpenReadStream();
            Console.WriteLine($"File name: {file.FileName}");
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = "uploaded_images"
            };
            Console.WriteLine($"File size: {file.Length}");
            Console.WriteLine($"File type: {file.ContentType}");
            Console.WriteLine($"File length: {file.Length}");
            Console.WriteLine($"File stream: {stream}");
            var result = await _cloudinary.UploadAsync(uploadParams);
            Console.WriteLine($"Upload result: {result.SecureUrl}");

            if (result.StatusCode == System.Net.HttpStatusCode.OK)
                return result.SecureUrl.ToString();
            if (result.Error != null)
            {
                Console.WriteLine($"Cloudinary error: {result.Error.Message}");
            }
            throw new Exception("Upload failed");
        }
    }   
}
