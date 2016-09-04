using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LightBlog.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace LightBlog.Models.Posts
{    
    public interface IPostRepository
    {
        IReadOnlyList<PostViewModel> GetAll();

        PagedPostsViewModel GetPaged(int page, int pageSize);

        IReadOnlyList<PostInformation> GetAllPostInformation(); 

        PostFindResult FindPost(int year, int month, string name);

        IReadOnlyList<PostViewModel> GetTopPosts(int number);

        Task CreatePost(IFormFile file);
    }

    public class PostRepository : IPostRepository
    {
        private readonly ILogger<PostRepository> logger;

        public PostRepository (ILogger<PostRepository> logger)
        {
            this.logger = logger;
        }

        public PagedPostsViewModel GetPaged(int page, int pageSize)
        {
            logger.LogInformation("Getting posts for page {page}", page);

            var fileInformation = GetAllPostInformation();

            var posts = fileInformation.OrderByDescending(x => x.Date)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new PostViewModel(x))
                .ToList();

            return new PagedPostsViewModel(posts, page, 
                (int)Math.Ceiling(fileInformation.Count / (double)pageSize));
        }

        public IReadOnlyList<PostViewModel> GetAll()
        {
            var fileInformation = GetAllPostInformation();

            return fileInformation.Select(x => new PostViewModel(x)).ToList();
        }

        public IReadOnlyList<PostInformation> GetAllPostInformation()
        {
            var postDirectory = GetPostDirectory();

            var files = Directory.GetFiles(postDirectory);

            return files.Select(x => new PostInformation(x))
                .Where(x => x.ValidPost).ToList();
        }

        public PostFindResult FindPost(int year, int month, string name)
        {
            var posts = GetAllPostInformation();

            var matchingPost = posts.FirstOrDefault(x => x.Date.Year == year
                && x.Date.Month == month
                && x.Name.IndexOf(name, StringComparison.OrdinalIgnoreCase) >= 0);

            if (matchingPost == null)
            {
                return new PostFindResult(null);
            }

            var viewModel = new PostViewModel(matchingPost);

            return new PostFindResult(viewModel);
        }

        public IReadOnlyList<PostViewModel> GetTopPosts(int number)
        {
            var posts = GetAllPostInformation();

            return posts.OrderByDescending(x => x.Date).Take(number)
                .Select(x => new PostViewModel(x))
                .ToList();
        }

        public async Task CreatePost(IFormFile file)
        {
            var path = Path.Combine(GetPostDirectory(), file.FileName);

            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
        }

        private static string GetPostDirectory()
        {
            var root = Directory.GetCurrentDirectory();

            var postDirectory = Path.Combine(root, "Posts");

            return postDirectory;
        }
    }
}