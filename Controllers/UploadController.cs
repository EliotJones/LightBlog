using System;
using System.IO;
using System.Threading.Tasks;
using LightBlog.Models;
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
        private readonly IUploadAuthentication authentication;

        public UploadController (ILogger<UploadController> logger,
            IPostRepository postRepository,
            IUploadAuthentication authentication)
        {
            this.logger = logger;
            this.postRepository = postRepository;
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

            return View();
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
                return View();       
            }

            if(!authentication.AuthenticateUploadAttempt(token, model.Password))
            {
                logger.LogInformation("Incorrect password");

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

            await postRepository.CreatePost(model.File);

            return RedirectToAction("Success");
        }

        public IActionResult Success()
        {
            return View();
        }
    }
}