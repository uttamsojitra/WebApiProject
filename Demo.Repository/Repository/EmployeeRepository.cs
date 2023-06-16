using Demo.Business.Interface;
using Demo.Entities.Data;
using Demo.Entities.Model;
using Demo.Entities.Model.ViewModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Business.Repository
{
    public class EmployeeRepository : BaseRepository<Employee>, IEmployeeRepository
    {
        private readonly UserDbcontext _userDbContext;

        public EmployeeRepository(UserDbcontext userDbcontext) : base(userDbcontext)
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

        public async Task<Employee> GetEmployee(long id)
        {
            return await ByIdAsync(id);
        }

        public async Task<Employee> AddEmployee(NewEmployee employee)
        {
            var ExistEmployee = await _userDbContext.Employees.FirstOrDefaultAsync(u => u.Email == employee.Email);

            if (ExistEmployee == null)
            {
                Employee newEmployee = new();
                newEmployee.Email = employee.Email;
                newEmployee.Department = employee.Department;
                newEmployee.FirstName = employee.FirstName;
                newEmployee.LastName = employee.LastName;
                newEmployee.Salary = employee.Salary;
                newEmployee.HireAt = employee.HireAt;
                newEmployee.Status = false;
                
                await AddAsync(newEmployee);

                return newEmployee;

            }
            return null;
        }
        
        public async Task AddEmployees(NewEmployee[] employees)
        {
            var newEmployeeEntities = employees.Select(employee =>
            {
                return new Employee
                {
                    Email = employee.Email,
                    Salary = employee.Salary,
                    FirstName = employee.FirstName,
                    LastName = employee.LastName,
                    Department = employee.Department,
                    HireAt = employee.HireAt,   
                    Status = false,                  
                };
            }).ToArray();
            await AddRangeAsync(newEmployeeEntities);
        }

        public async Task<Employee> UpdateEmployee(UpdateEmployeeViewModel employee)
        {
            var updateEmployee = await ByIdAsync(employee.EmployeeId);
            if (updateEmployee != null)
            {
                updateEmployee.Email = employee.Email;
                updateEmployee.FirstName = employee.FirstName;
                updateEmployee.LastName = employee.LastName;
                updateEmployee.Salary = employee.Salary;
                updateEmployee.Department = employee.Department;
                return await UpdateAsync(updateEmployee);
            }
            return updateEmployee;
        }

        public async Task<List<EmployeeNotFoundViewModel>> UpdateEmployees(UpdateEmployeeViewModel[] employees)
        {
            var employeeIdsNotFound = new List<EmployeeNotFoundViewModel>();
            var existingEmployees = await GetByIdsAsync(employees.Select(u => u.EmployeeId).ToList());

            var updatedEmployees = employees.Select(user =>
            {
                var existingEmployee = existingEmployees.FirstOrDefault(u => u.EmployeeId == user.EmployeeId);
                if (existingEmployee != null)
                {
                    existingEmployee.Email = user.Email;
                    existingEmployee.FirstName = user.FirstName;
                    existingEmployee.LastName = user.LastName;
                    existingEmployee.Department = user.Department;
                    existingEmployee.Status = user.Status;
                    existingEmployee.Salary = user.Salary;
                }
                return existingEmployee;
            }).Where(user => user != null).ToArray();


            if (updatedEmployees.Length > 0)
            {
                await UpdateRangeAsync(updatedEmployees);
            }

            employeeIdsNotFound = employees.Where(employee => updatedEmployees.All(e => e.EmployeeId != employee.EmployeeId))
                                  .Select(user => new EmployeeNotFoundViewModel
                                  {
                                      EmployeeId = user.EmployeeId,
                                      EmployeeName = user.FirstName + " " + user.LastName
                                  })
                                  .ToList();

            return employeeIdsNotFound;
        }

        public async Task<List<Employee>> GetByIdsAsync(List<long> employeeIds)
        {
            return await _userDbContext.Employees.Where(u => employeeIds.Contains(u.EmployeeId)).ToListAsync();
        }

        public async Task<bool> RemoveEmployee(long id)
        {
            Employee employee = await ByIdAsync(id);
            if (employee != null)
            {
                return await RemoveAsync(employee);
            }
            return false;
        }

    }
}
