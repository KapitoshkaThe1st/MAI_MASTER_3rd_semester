using ApplicationAPI.Entities;
using ApplicationAPI.Requests;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationAPI.Repository
{
    public class UserProfilessRepository : Repository<UserProfile>, IUsersProfilesRepository
    {
        public UserProfilessRepository(IMongoDbSettings settings) : base(settings) { }

        public Task<IEnumerable<UserProfile>> GetUserProfiles(UsersRequestParameters parameters)
        {
            return GetWithParameters(parameters);
        }

        public Task<UserProfile> GetUserProfileById(string id)
        {
            return FindByIdAsync(id);
        }

        public Task<UserProfile> GetUserProfileById(ObjectId id)
        {
            return FindByIdAsync(id);
        }

        public Task CreateUserProfile(UserProfile user)
        {
            return InsertOneAsync(user);
        }

        public Task DeleteUserProfile(string id)
        {
            return DeleteByIdAsync(id);
        }
        public Task UpdateUserProfile(UserProfile user)
        {
            return ReplaceOneAsync(user);
        }

        public Task<UserProfile> AddSubscription(string userId, string subscriptionTargetId)
        {
            var uid = new ObjectId(userId);

            var filter = Builders<UserProfile>
                .Filter.Eq(user => user.Id, uid);

            var update = Builders<UserProfile>.Update
                .AddToSet(e => e.SubscriptionsIds, new ObjectId(subscriptionTargetId));

            return _collection.FindOneAndUpdateAsync(
                filter, 
                update,
                options: new FindOneAndUpdateOptions<UserProfile, UserProfile> 
                {
                    ReturnDocument = ReturnDocument.After
                });
        }

        public Task<IEnumerable<ObjectId>> GetSubscriptions(string userId)
        {
            var uid = new ObjectId(userId);
            return Task.Run(() => _collection.Find(user => user.Id == uid).FirstOrDefault().SubscriptionsIds);
        }

        public Task<ObjectId?> GetSubscription(string userId, int index)
        {
            var uid = new ObjectId(userId);
            return Task.Run<ObjectId?>(() =>
            {
                var subscriptions = _collection.Find(user => user.Id == uid).FirstOrDefault().SubscriptionsIds;
                if (index < 0 || index >= subscriptions.Count())
                {
                    return null;
                }
                return subscriptions.ElementAt(index);
            });
        }
    }
}
