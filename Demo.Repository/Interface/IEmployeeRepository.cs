using Demo.Entities.Model;
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

        public Task<Employee> GetEmployeeById(long id);
    }
}
