using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace LightBlog.ViewModels
{
    public class UploadFileViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Upload)]
        public IFormFile File { get; set; }
    }
}