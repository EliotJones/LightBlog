using LightBlog.ViewModels;

namespace LightBlog.Models.Posts
{
    public class PostFindResult
    {
        public PostViewModel Post { get; }

        public bool IsFound { get; }

        public PostFindResult (PostViewModel post)
        {
            Post = post;
            IsFound = post != null;
        }
    }
}