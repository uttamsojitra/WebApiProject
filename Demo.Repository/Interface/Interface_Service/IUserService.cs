using Demo.Entities.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Business.Interface.Interface_Service
{
    public interface IUserService
    {
        public Task<User> GetUserById(int id);
        public Task<IEnumerable<User>> GetAllUsers(int pageNumber, int pageSize);
        public int GetTotalPages(int pageSize);
        public int GetTotalUsersCount();
        public Task CreateUser(User user);
        public Task UpdateUser(User user);
        public Task<bool> DeleteUser(int id);
    }
}
