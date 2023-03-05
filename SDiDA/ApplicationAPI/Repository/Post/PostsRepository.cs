using ApplicationAPI.Entities;
using ApplicationAPI.Requests;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ApplicationAPI.Repository
{
    public class PostsRepository : Repository<Post>, IPostsRepository
    {
        public PostsRepository(IMongoDbSettings settings) : base(settings) { }

        public Task<IEnumerable<Post>> GetPosts(PostsRequestParameters parameters)
        {
            return GetWithParameters(parameters);
        }

        public Task<IEnumerable<Post>> GetPostsByIds(IEnumerable<string> ids)
        {
            return GetPostsByIds(ids.Select(id => new ObjectId(id)));
        }

        public Task<IEnumerable<Post>> GetPostsByIds(IEnumerable<ObjectId> ids)
        {
            return FindAsync(post => ids.Contains(post.Id));
        }

        public Task<IEnumerable<Post>> GetPostsByAuthorsIds(IEnumerable<string> authorsIds)
        {
            return GetPostsByAuthorsIds(authorsIds.Select(id => new ObjectId(id)));
        }

        public Task<IEnumerable<Post>> GetPostsByAuthorsIds(IEnumerable<ObjectId> authorsIds)
        {
            return FindAsync(post => authorsIds.Contains(post.AuthorId));
        }

        public Task<IEnumerable<Post>> GetLatestPostsAsync(IEnumerable<ObjectId> authorsIds, PostsRequestParameters parameters)
        {
            var filterDefinition = Builders<Post>.Filter.In(post => post.AuthorId, authorsIds);

            return Task.Run(() => _collection
                .Find(filterDefinition)
                .Skip(parameters.ToSkip)
                .Limit(parameters.PageSize)
                .SortByDescending(post => post.AuthorId).ToEnumerable());

            //return GetWithParameters(post => authorsIds.Contains(post.AuthorId), post => post.CreatedAt, parameters);
        }

        public Task<IEnumerable<Post>> GetLatestPostsAsync(IEnumerable<string> authorsIds, PostsRequestParameters parameters)
        {
            return GetLatestPostsAsync(authorsIds.Select(id => new ObjectId(id)), parameters);
        }

        public Task<Post> GetPostById(string id)
        {
            return FindByIdAsync(id);
        }

        public Task CreatePost(Post user)
        {
            return InsertOneAsync(user);
        }

        public Task DeletePost(string id)
        {
            return DeleteByIdAsync(id);
        }
        public Task UpdatePost(Post user)
        {
            return ReplaceOneAsync(user);
        }
    }
}
