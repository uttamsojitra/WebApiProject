using Demo.Entities.Model;
using Demo.Entities.Model.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Business.Interface
{
    public interface IEmployeeRepository : IBaseRepository<Employee>
    {
        public Task<List<string>> GetEmployeeByName();
        public Task<List<Employee>> GetEmployeesAsync();
        public Task<List<Employee>> GetByIdsAsync(List<long> employeeIds);
        public Task<Employee> GetEmployeeEmail(string email);

        // Sql Queries
        public Task<List<DepartmentViewModel>> EmpByDepartment();
        public Task<List<EmployeeViewModel>> EmployeeFromHR();
        public Task<string> GetHiringDates();
        public Task<string> GetAllFirstName();
    }
}
