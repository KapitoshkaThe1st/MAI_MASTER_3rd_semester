using MongoDB.Bson;

namespace ApplicationAPI.DTOs
{
    public record PostDto(ObjectId PostId, ObjectId AuthorId, string Text);
}
