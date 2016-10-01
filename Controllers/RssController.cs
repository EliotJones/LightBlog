using LightBlog.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LightBlog.Controllers
{
    public class RssController : Controller
    {
        private readonly ILogger<RssController> logger;
        private readonly IRssFeedFactory feedFactory;

        public RssController(IRssFeedFactory feedFactory,
            ILogger<RssController> logger)
        {
            this.logger = logger;
            this.feedFactory = feedFactory;
        }

        public IActionResult Index()
        {
            logger.LogInformation("Getting RSS feed.");

            var feed = feedFactory.GetFeed();

            return Ok(feed);
        }
    }
}