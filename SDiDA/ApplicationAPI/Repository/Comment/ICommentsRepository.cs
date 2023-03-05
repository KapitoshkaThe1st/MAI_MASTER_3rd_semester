using ApplicationAPI.Entities;
using ApplicationAPI.Requests;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApplicationAPI.Repository
{
    public interface ICommentsRepository
    {
        public Task<IEnumerable<Comment>> GetComments(CommentsRequestParameters parameters);
        public Task<Comment> GetCommentById(string id);
        public Task CreateComment(Comment comment);
        public Task DeleteComment(string id);
        public Task UpdateComment(Comment comment);
    }
}
