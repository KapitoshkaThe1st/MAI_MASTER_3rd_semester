using ApplicationAPI.Entities;
using ApplicationAPI.Requests;
using MongoDB.Bson;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApplicationAPI.Repository
{
    public interface IUsersProfilesRepository
    {
        public Task<IEnumerable<UserProfile>> GetUserProfiles(UsersRequestParameters parameters);
        public Task<UserProfile> GetUserProfileById(string id);
        public Task<UserProfile> GetUserProfileById(ObjectId id);
        public Task CreateUserProfile(UserProfile user);
        public Task DeleteUserProfile(string id);
        public Task UpdateUserProfile(UserProfile user);
        public Task<UserProfile> AddSubscription(string userId, string subscriptionTargetId);
        public Task<IEnumerable<ObjectId>> GetSubscriptions(string userId);
        public Task<ObjectId?> GetSubscription(string userId, int index);
    }
}
