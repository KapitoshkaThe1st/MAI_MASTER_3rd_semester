using ApplicationAPI.Repository;
using MongoDB.Bson;

namespace ApplicationAPI.Entities
{
    public class TextContent : Document
    {
        public ObjectId AuthorId { get; set; }
        public string Text { get; set; }
    }
}
