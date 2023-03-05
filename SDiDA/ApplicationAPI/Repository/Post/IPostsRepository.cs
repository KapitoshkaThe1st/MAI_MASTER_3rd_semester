using ApplicationAPI.Entities;
using ApplicationAPI.Requests;
using MongoDB.Bson;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApplicationAPI.Repository
{
    public interface IPostsRepository
    {
        public Task<IEnumerable<Post>> GetPosts(PostsRequestParameters parameters);
        public Task<Post> GetPostById(string id);
        public Task CreatePost(Post user);
        public Task DeletePost(string id);
        public Task UpdatePost(Post user);
        public Task<IEnumerable<Post>> GetPostsByIds(IEnumerable<string> ids);
        public Task<IEnumerable<Post>> GetPostsByIds(IEnumerable<ObjectId> ids);
        public Task<IEnumerable<Post>> GetPostsByAuthorsIds(IEnumerable<string> authorsIds);
        public Task<IEnumerable<Post>> GetPostsByAuthorsIds(IEnumerable<ObjectId> authorsIds);
        public Task<IEnumerable<Post>> GetLatestPostsAsync(IEnumerable<ObjectId> authorsIds, PostsRequestParameters parameters);
        public Task<IEnumerable<Post>> GetLatestPostsAsync(IEnumerable<string> authorsIds, PostsRequestParameters parameters);
    }
}
