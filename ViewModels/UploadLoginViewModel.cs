using System.ComponentModel.DataAnnotations;

namespace LightBlog.ViewModels
{
    public class UploadLoginViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}