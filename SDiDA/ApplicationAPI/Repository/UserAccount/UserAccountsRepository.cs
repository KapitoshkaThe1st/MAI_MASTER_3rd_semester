using ApplicationAPI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationAPI.Repository
{
    public class UserAccountsRepository : Repository<UserAccount>, IUsersAccountsRepository
    {
        public UserAccountsRepository(IMongoDbSettings settings) : base(settings) { }

        public Task CreateUserAccount(UserAccount userAccount)
        {
            return InsertOneAsync(userAccount);
        }

        public Task<UserAccount> GetUserAccountByLogin(string login)
        {
            return FindOneAsync(u => u.Login == login);
        }

        public Task<UserAccount> GetUserAccountById(string id)
        {
            return FindByIdAsync(id);
        }
    }
}
