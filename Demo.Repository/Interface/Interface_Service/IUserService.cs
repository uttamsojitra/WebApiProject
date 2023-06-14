using Demo.Entities.Model;
using Demo.Entities.Model.ViewModel;
using Demo.Entities.Models;
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
        public Task<byte[]> ExportUsersDataToExcel();
        public Task<byte[]> ExportUsersDataToPDF();
        public Task<byte[]> ExportUsersDataToWord();
        public Task<IEnumerable<User>> GetAllUsers(int pageNumber, int pageSize);
        public int GetTotalPages(int pageSize);
        public int GetTotalUsersCount();
        public Task<User> CreateUser(UserSignUpViewModel user);
        public Task<User> GetEmailAndToken(string email, string token);
        public Task<User> GetUserStatus(string email, string token);
        public Task<User> UpdateUser(User user);
        public Task<bool> DeleteUser(int id);
        public Task<List<DepartmentViewModel>> EmployeeByDept();
        public Task<List<EmployeeViewModel>> EmployeeFromHR();
        public Task<string> GerAllHireDates();
        public Task<string> GetEmpFirstName();
        public Task<StoreUsersResponseModel> StoreUsersFromExcel(Stream fileStream);
        public Task<List<string>> GetAllSkills();

        //---  Employee Repo ----
        public Task<List<string>> GetEmployeesName();
        public Task<Dictionary<string, int>> GetEmployeeCountByDepartment();
        public Task<Employee> GetEmployeeById(int id);
    }
}
