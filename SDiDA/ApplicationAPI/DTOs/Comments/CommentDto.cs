using MongoDB.Bson;

namespace ApplicationAPI.DTOs
{
    public record CommentDto(ObjectId CommentId, ObjectId PostId, ObjectId AuthorId, string Text);
}
