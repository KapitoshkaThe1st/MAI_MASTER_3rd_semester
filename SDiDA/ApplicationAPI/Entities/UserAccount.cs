using ApplicationAPI.Repository;
using MongoDB.Bson;

namespace ApplicationAPI.Entities
{
    [BsonCollection("user_accounts")]
    public class UserAccount : Document
    {
        public string Login { get; set; }
        public string PasswordHash { get; set; }
        public ObjectId ProfileId { get; set; }
    }
}
