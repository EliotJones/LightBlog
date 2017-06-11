using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

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

        public string Type { get; set; }

        public SelectList Types { get; set; }

        public string Folder { get; set; }

        public UploadType SelectedType => (UploadType)Enum.Parse(typeof(UploadType), Type);

        public UploadFileViewModel()
        {
          var local = new List<SelectListItem>
          {
              new SelectListItem
              {
                  Text = "Post",
                  Value = UploadType.Post.ToString()
              },
              new SelectListItem
              {
                  Text = "Image",
                  Value = UploadType.Image.ToString()
              }
          };

          Types = new SelectList(local, "Value", "Text");
        }
    }
}