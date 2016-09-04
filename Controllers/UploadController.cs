using System;
using LightBlog.Models;
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

        public UploadController (ILogger<UploadController> logger,
            IOptions<UploadOptions> options)
        {
            this.logger = logger;
            this.options = options;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(UploadFileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();       
            }

            logger.LogInformation("Uploading file. Name: {0}, Content Type: {1}", model.File.FileName,
                model.File.ContentType);

            if (!string.Equals(model.File.ContentType, "text/plain", 
                StringComparison.CurrentCultureIgnoreCase))
            {
                logger.LogInformation("File format was incorrect.");

                return View();
            }

            logger.LogInformation("Password = " + options.Value.UploadPassword);

            if(model.Password != options.Value.UploadPassword)
            {
                logger.LogInformation("Incorrect password");

                return View();
            }

            return RedirectToAction("Success");
        }

        public IActionResult Success()
        {
            return View();
        }
    }
}