using Demo.Business.Interface;
using Demo.Entities.Data;
using Demo.Entities.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Business.Repository
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly UserDbcontext _userDbContext;

        public EmployeeRepository(UserDbcontext userDbcontext)
        {
            _userDbContext = userDbcontext; 
        }

        public async Task<List<string>> GetEmployeeByName()
        {
            return await _userDbContext.Employees.Select(emp => emp.FirstName +" "+emp.LastName).ToListAsync();         
        }

        public async Task<List<Employee>> GetEmployeesAsync()
        {
            return await _userDbContext.Employees.ToListAsync();
        }

        public async Task<Employee> GetEmployeeById(long id)
        {
            var employee = await _userDbContext.Employees.FindAsync(id);
            return employee;
        }
    }
}
