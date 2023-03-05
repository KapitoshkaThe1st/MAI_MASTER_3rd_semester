using ApplicationAPI.Repository;
using MongoDB.Bson;

namespace ApplicationAPI.Entities
{
    [BsonCollection("comments")]
    public class Comment : TextContent
    {
        public ObjectId PostId { get; set; }
    }
}
