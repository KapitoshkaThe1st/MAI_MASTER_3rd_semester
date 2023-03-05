using MongoDB.Bson;

namespace ApplicationAPI.DTOs
{
    public record CommentDto(ObjectId PostId, ObjectId AuthorId, string Text);
}
