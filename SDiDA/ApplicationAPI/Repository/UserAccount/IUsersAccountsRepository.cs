using ApplicationAPI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationAPI.Repository
{
    public interface IUsersAccountsRepository
    {
        Task<UserAccount> GetUserAccountByLogin(string login);
        Task CreateUserAccount(UserAccount userAccount);
        Task<UserAccount> GetUserAccountById(string id);
    }
}
