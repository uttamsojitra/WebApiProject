using Demo.Business.Interface;
using Demo.Entities.Model;
using Demo.Entities.Model.ViewModel;
using Demo.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Repository.Interface
{
    public interface IUserRepository : IBaseRepository<User>
    {
        public Task<List<User>> GetUserList();       
        public Task<User> GetUserEmail(string email);
        public Task<List<User>> GetUsersData();
        public Task<List<string>> GetSkills();      
        public Task<User> GetUserByEmailAndToken(string email, string token);
        public Task<User> GetUserStatus(string email, string token);
        public int GetTotalUsersCount();
        public Task<User> GetAuthUser(string Email, string Password);
        public Task<List<User>> GetByIdsAsync(List<long> userIds);
    }
}
