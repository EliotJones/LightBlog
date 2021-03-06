using LightBlog.Models.Posts;
using Microsoft.AspNetCore.Mvc;

namespace LightBlog.Controllers
{
    public class HomeController : Controller
    {
        private readonly IPostRepository postRepository;

        public HomeController (IPostRepository postRepository)
        {
            this.postRepository = postRepository;
        }

        public IActionResult Index(int page = 1)
        {
            var posts = postRepository.GetPaged(page, 3);

            return View(posts);
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
