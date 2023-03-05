using ApplicationAPI.Repository;
using MongoDB.Bson;

namespace ApplicationAPI.Entities
{
    [BsonCollection("posts")]
    public class Post : TextContent { }
}
