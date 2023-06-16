using Demo.Entities.Model;
using Demo.Entities.Model.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Business.Interface
{
    public interface IEmployeeRepository
    {
        public Task<List<string>> GetEmployeeByName();
        public Task<List<Employee>> GetEmployeesAsync();

        public Task<Employee> GetEmployee(long id);
        public Task<bool> RemoveEmployee(long id);
        public Task<Employee> AddEmployee(NewEmployee employee);
        public Task<Employee> UpdateEmployee(UpdateEmployeeViewModel employee);
        public Task AddEmployees(NewEmployee[] employees);
        public Task<List<EmployeeNotFoundViewModel>> UpdateEmployees(UpdateEmployeeViewModel[] employees);
    }
}
