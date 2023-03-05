using MongoDB.Bson;

namespace ApplicationAPI.DTOs
{
    public record PostDto(ObjectId AuthorId, string Text);
}
