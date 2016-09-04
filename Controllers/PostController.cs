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
            logger.LogInformation("Getting post year {year}, month {month}, name: {name}",
                year,
                month,
                name);

            var post = postRepository.FindPost(year, month, name);

            if (!post.IsFound)
            {
                logger.LogInformation("Post not found!");
                
                return RedirectToAction("Index", "Home");
            }

            return View(post.Post);
        }
    }
}