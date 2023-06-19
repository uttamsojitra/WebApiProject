using Demo.Entities.Model;
using Demo.Entities.Model.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Business.Interface.Interface_Service
{
    public interface IEmployeeService
    {
        public Task<List<string>> GetEmployeesName();
        public Task<Dictionary<string, int>> GetEmployeeCountByDepartment();
        public Task<Employee> GetEmployeeById(int id);
        public Task<bool> DeleteEmployee(int id);
        public Task<Employee> CreateEmployee(NewEmployee employee);
        public Task AddRangeEmployees(NewEmployee[] NewEmployees);
        public Task<Employee> UpdateEmployee(UpdateEmployeeViewModel employee);
        public Task<List<EmployeeNotFoundViewModel>> UpdateEmployees(UpdateEmployeeViewModel[] employees);

        // SQl queries
        public Task<List<DepartmentViewModel>> EmployeeByDept();
        public Task<List<EmployeeViewModel>> EmployeeFromHR();
        public Task<string> GerAllHireDates();
        public Task<string> GetEmpFirstName();
    }
}
