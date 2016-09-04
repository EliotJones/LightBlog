using System.Threading.Tasks;
using LightBlog.Models.Posts;
using Microsoft.AspNetCore.Mvc;

namespace LightBlog.ViewComponents
{
    public class SideBarViewComponent : ViewComponent
    {
        private readonly IPostRepository postRepository;

        public SideBarViewComponent(IPostRepository postRepository)
        {
            this.postRepository = postRepository;
        }

        public Task<IViewComponentResult> InvokeAsync()
        {
            var topPosts = postRepository.GetTopPosts(5);

            return Task.FromResult((IViewComponentResult)View(topPosts));
        }
    }
}