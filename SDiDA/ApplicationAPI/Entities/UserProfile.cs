using ApplicationAPI.Repository;
using MongoDB.Bson;
using System.Collections.Generic;

namespace ApplicationAPI.Entities
{
    [BsonCollection("user_profiles")]
    public class UserProfile : Document
    {
        public string Username { get; set; }
        public string Status { get; set; }
        public IEnumerable<ObjectId> SubscriptionsIds { get; set; } = new List<ObjectId>();
    }
}
