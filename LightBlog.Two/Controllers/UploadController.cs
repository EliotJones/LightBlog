using System;
using System.Threading.Tasks;
using LightBlog.Models;
using LightBlog.Models.Images;
using LightBlog.Models.Posts;
using LightBlog.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LightBlog.Controllers
{
    public class UploadController : Controller
    {
        private readonly ILogger<UploadController> logger;
        private readonly IPostRepository postRepository;
        private readonly IImageRepository imageRepository;
        private readonly IUploadAuthentication authentication;

        public UploadController (ILogger<UploadController> logger,
            IPostRepository postRepository,
            IImageRepository imageRepository,
            IUploadAuthentication authentication)
        {
            this.logger = logger;
            this.postRepository = postRepository;
            this.imageRepository = imageRepository;
            this.authentication = authentication;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(UploadLoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var value = authentication.AuthenticateStepOne(model.Password);

            if (string.IsNullOrWhiteSpace(value))
            {
                logger.LogInformation("Failed to login.");

                return View();
            }

            HttpContext.Response.Cookies.Append("__token", value);

            return RedirectToAction("Index");
        }

        public IActionResult Index()
        {
            var token = HttpContext.Request.Cookies["__token"]?.ToString();
            
            if (string.IsNullOrWhiteSpace(token) || !authentication.HasValidToken(token))
            {
                return RedirectToAction("Login");
            }

            return View(new UploadFileViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Index(UploadFileViewModel model)
        {
            var token = HttpContext.Request.Cookies["__token"]?.ToString();

            if (!authentication.HasValidToken(token))
            {
                return RedirectToAction("Login");
            }

            if (!ModelState.IsValid)
            {
                return View(new UploadFileViewModel());       
            }

            if(!authentication.AuthenticateUploadAttempt(token, model.Password))
            {
                logger.LogInformation("Incorrect password");

                return View(new UploadFileViewModel());
            }

            logger.LogInformation("Uploading file. Name: {0}, Content Type: {1}", 
                model.File.FileName,
                model.File.ContentType);

            if (!ValidUpload(model))
            {
                logger.LogInformation("File format was incorrect. {0}", model.File.ContentType);

                return View(new UploadFileViewModel());
            }

            switch (model.SelectedType)
            {
                case UploadType.Post:
                    await postRepository.CreatePost(model.File);
                    break;
                case UploadType.Image:
                    await imageRepository.UploadImage(model.Folder, model.File);
                    break;
                default:
                    break;
            }

            return RedirectToAction("Success");
        }

        private bool ValidUpload(UploadFileViewModel model)
        {
            logger.LogInformation("Upload type: {0}, Selected Type: {1}", 
                model.File.ContentType,
                model.Type);

            if (string.Equals(model.File.ContentType, "text/plain", 
                StringComparison.CurrentCultureIgnoreCase)
                && model.SelectedType == UploadType.Post)
            {
                return true;
            }

            if(model.SelectedType == UploadType.Image
                && model.File.ContentType.StartsWith("image"))
            {
                return true;
            }            

            return false;
        }

        public IActionResult Success()
        {
            return View();
        }
    }
}