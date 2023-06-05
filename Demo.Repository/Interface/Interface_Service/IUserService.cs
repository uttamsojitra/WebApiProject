using Demo.Entities.Model;
using Demo.Entities.Model.ViewModel;
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
        public Task<User> CreateUser(UserSignUpViewModel user);
        public Task<User> GetEmailAndToken(string email, string token);
        public Task<User> GetUserStatus(string email, string token);
        public Task UpdateUser(User user);
        public Task<bool> DeleteUser(int id);

        public  Task<List<DepartmentViewModel>> EmployeeByDept();
        public  Task<List<EmployeeViewModel>> EmployeeFromHR();
        public  Task<string> GerAllHireDates();
        public  Task<string> GetEmpFirstName();
    }
}
