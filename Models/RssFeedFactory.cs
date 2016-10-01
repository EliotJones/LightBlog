using System;
using System.Linq;
using LightBlog.Models.Posts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Logging;
using WilderMinds.RssSyndication;

namespace LightBlog.Models
{
    internal class RssFeedFactory : IRssFeedFactory
    {
        private readonly IPostRepository postRepository;
        private readonly IUrlHelper urlHelper;
        private readonly ILogger<RssFeedFactory> logger;

        public RssFeedFactory(IPostRepository postRepository,
         IUrlHelperFactory urlHelperFactory,
         IActionContextAccessor actionContextAccessor,
         ILogger<RssFeedFactory> logger)
        {
            this.postRepository = postRepository;
            this.urlHelper = urlHelperFactory.GetUrlHelper(actionContextAccessor.ActionContext);
            this.logger = logger;
        }

        public string GetFeed()
        {
            var posts = postRepository.GetAll();

            var feed = new Feed
            {
                Title = "Light Blog ",
                Description = "Site Description",
                Link = new Uri(urlHelper.Link("default", new { action = "Index", controller = "Home" }))
            };

            var feedPosts = posts.Select(x =>
            {
                var url = urlHelper.Link("post", new { year = x.Year, month = x.Month, name = x.Name });

                return new Item
                {
                    Title = x.Title,
                    Body = x.Text,
                    Link = new Uri(url),
                    Permalink = url,
                    PublishDate = x.Date,
                    Author = new Author { Name = "Eliot Jones" }
                };
            })
            .OrderByDescending(x => x.PublishDate)
            .ToList();

            logger.LogInformation($"Found {feedPosts.Count} posts");

            feed.Items = feedPosts;

            var rss = feed.Serialize();

            return rss;
        }
    }

    public interface IRssFeedFactory
    {
        string GetFeed();
    }
}