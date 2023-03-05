using ApplicationAPI.Entities;
using ApplicationAPI.Requests;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApplicationAPI.Repository
{
    public class CommentsRepository : Repository<Comment>, ICommentsRepository
    {
        public CommentsRepository(IMongoDbSettings settings) : base(settings) { }

        public Task CreateComment(Comment comment)
        {
            return InsertOneAsync(comment);
        }

        public Task DeleteComment(string id)
        {
            return DeleteByIdAsync(id);
        }

        public Task<Comment> GetCommentById(string id)
        {
            return FindByIdAsync(id);
        }
        public Task<IEnumerable<Comment>> GetCommentsForPost(string postId)
        {
            var pid = new ObjectId(postId);
            return FindAsync(comment => comment.PostId == pid);
        }
        public Task<IEnumerable<Comment>> GetCommentsOfUset(string userId)
        {
            var uid = new ObjectId(userId);
            return FindAsync(comment => comment.PostId == uid);
        }

        public Task<IEnumerable<Comment>> GetComments(CommentsRequestParameters parameters)
        {
            return GetWithParameters(parameters);
        }

        public Task UpdateComment(Comment comment)
        {
            throw new NotImplementedException();
        }
    }
}
