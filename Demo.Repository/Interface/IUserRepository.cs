using Demo.Entities.Model;
using Demo.Entities.Model.ViewModel;
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
        public Task<List<User>> GetUsersData();


        public Task<User> AddUser(UserSignUpViewModel user);
        public Task<User> GetUserByEmailAndToken(string email, string token);
        public Task<User> GetUserStatus(string email, string token);
        public Task UpdateUser(User user);
        public Task<bool> RemoveUser(long id);
        public int GetTotalUsersCount();

        public Task<User> GetAuthUser(string Email, string Password);
        public Task<List<DepartmentViewModel>> EmpByDepartment();
        public Task<List<EmployeeViewModel>> EmployeeFromHR();
        public Task<string> GetHiringDates();
        public Task<string> GetAllFirstName();
    }
}
