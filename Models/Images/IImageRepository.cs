using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace LightBlog.Models.Images
{
    public interface IImageRepository
    {
        Task UploadImage(string folder, IFormFile file);
    }    

    public class ImageRepository : IImageRepository
    {
        private readonly ILogger<ImageRepository> logger;

        public ImageRepository (ILogger<ImageRepository> logger)
        {
            this.logger = logger;
        }

        public async Task UploadImage(string folder, IFormFile file)
        {
            var root = Directory.GetCurrentDirectory();

            var directory = Path.Combine(root, "wwwroot", "images");

            if (!string.IsNullOrWhiteSpace(folder))
            {
                directory = Path.Combine(directory, folder);
            }

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            directory = Path.Combine(directory, file.FileName);

            using (var fileStream = new FileStream(directory, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
        }
    }
}
