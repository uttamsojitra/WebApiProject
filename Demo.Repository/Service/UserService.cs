using Demo.Business.Exception;
using Demo.Business.Interface.Interface_Service;
using Demo.Entities.Model;
using Demo.Entities.Model.ViewModel;
using Demo.Repository.Interface;
using Demo.Repository.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Repository.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        //-------    User CRUD operation    -------
        public async Task<User> GetUserById(int id)
        {
            return await _userRepository.GetUser(id);
        }

        public async Task<IEnumerable<User>> GetAllUsers(int pageNumber, int pageSize)
        {
            var users = await _userRepository.GetUserList();
            var usersPerPage = users.Skip((pageNumber - 1) * pageSize).Take(pageSize);
            return usersPerPage;
        }

        public int GetTotalPages(int pageSize)
        {
            var totalUsers = _userRepository.GetTotalUsersCount();
            return (int)Math.Ceiling((decimal)totalUsers / pageSize);
        }

        public int GetTotalUsersCount()
        {
            return  _userRepository.GetTotalUsersCount();
        }

        public async Task CreateUser(User user)
        {
            await _userRepository.AddUser(user);
        }

        public async Task UpdateUser(User user)
        {
            await _userRepository.UpdateUser(user);
        }

        public async Task<bool> DeleteUser(int id)
        {
            var result = await _userRepository.RemoveUser(id);
            if (!result)
            {
               return false;
            }
            return true;    
        }

        //-------    Sql queries on Employee table    -------
        public async Task<List<DepartmentViewModel>> EmployeeByDept()
        {
           return await _userRepository.EmpByDepartment();
        } 
        public async Task<List<EmployeeViewModel>> EmployeeFromHR()
        {
           return await _userRepository.EmployeeFromHR();
        }
        public async Task<string> GerAllHireDates()
        {
            return await _userRepository.GetHiringDates();
        }
        public async Task<string> GetEmpFirstName()
        {
            return await _userRepository.GetAllFirstName();
        }
    }
}
