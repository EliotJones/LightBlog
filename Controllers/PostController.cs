using LightBlog.Models.Posts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LightBlog.Controllers
{
    public class PostController : Controller
    {
        private readonly IPostRepository postRepository;
        private readonly ILogger<PostController> logger;

        public PostController (IPostRepository postRepository,
            ILogger<PostController> logger)
        {
            this.postRepository = postRepository;
            this.logger = logger;
        }

        public IActionResult Index(int year, int month, string name)
        {
            logger.LogInformation("Getting post year {year}, month {month}, name: {name}");

            return View();
        }
    }
}