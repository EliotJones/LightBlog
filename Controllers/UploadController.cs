using System;
using System.IO;
using System.Threading.Tasks;
using LightBlog.Models;
using LightBlog.Models.Posts;
using LightBlog.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LightBlog.Controllers
{
    public class UploadController : Controller
    {
        private readonly ILogger<UploadController> logger;
        private readonly IOptions<UploadOptions> options;
        private readonly IPostRepository postRepository;

        public UploadController (ILogger<UploadController> logger,
            IOptions<UploadOptions> options,
            IPostRepository postRepository)
        {
            this.logger = logger;
            this.options = options;
            this.postRepository = postRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(UploadFileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();       
            }

            logger.LogInformation("Uploading file. Name: {0}, Content Type: {1}", 
                model.File.FileName,
                model.File.ContentType);

            if (!string.Equals(model.File.ContentType, "text/plain", 
                StringComparison.CurrentCultureIgnoreCase))
            {
                logger.LogInformation("File format was incorrect.");

                return View();
            }

            if(model.Password != options.Value.UploadPassword)
            {
                logger.LogInformation("Incorrect password");

                return View();
            }

            await postRepository.CreatePost(model.File);

            return RedirectToAction("Success");
        }

        public IActionResult Success()
        {
            return View();
        }
    }
}