using Demo.Entities.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Repository.Interface
{
    public interface IUserRepository
    {
        public Task<List<User>> GetUserList();
        public Task<User> GetUser(long id);
        public Task AddUser(User user);
        public Task UpdateUser(User user);
        public Task<bool> RemoveUser(long id);
        public int GetTotalUsersCount();

        public Task<User> GetAuthUser(string FirstName, string Password);
    }
}
