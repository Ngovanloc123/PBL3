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
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(file.FileName, stream),
            Folder = "uploaded_images"
        };

        var result = await _cloudinary.UploadAsync(uploadParams);

        if (result.StatusCode == System.Net.HttpStatusCode.OK)
            return result.SecureUrl.ToString();

        throw new Exception("Upload failed");
    }
}
}
