using Demo.Business.Interface;
using Demo.Business.Interface.Interface_Service;
using Demo.Entities.Model;
using Demo.Entities.Model.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserDemo.Data.migrations;

namespace Demo.Business.Service
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeeService(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;   
        }

        public async Task<List<string>> GetEmployeesName()
        {
            return await _employeeRepository.GetEmployeeByName();
        }

        public async Task<Dictionary<string, int>> GetEmployeeCountByDepartment()
        {
            var employees = await _employeeRepository.GetEmployeesAsync();
            var countByDepartment = employees.GroupBy(e => e.Department)
                .ToDictionary(g => g.Key, g => g.Count());

            return countByDepartment;
        }

        //-- CRUD operation ---

        public async Task<Employee> GetEmployeeById(int id)
        {
            return await _employeeRepository.ByIdAsync(id);
        }

        public async Task<Employee> CreateEmployee(NewEmployee newEmployee)
        {
            var existEmployee = await _employeeRepository.GetEmployeeEmail(newEmployee.Email);

            if (existEmployee == null)
            {
                Employee addEmployee = new();
                newEmployee.Email = newEmployee.Email;
                newEmployee.Department = newEmployee.Department;
                newEmployee.FirstName = newEmployee.FirstName;
                newEmployee.LastName = newEmployee.LastName;
                newEmployee.Salary = newEmployee.Salary;
                newEmployee.HireAt = newEmployee.HireAt;
                newEmployee.Status = false;

                await _employeeRepository.AddAsync(addEmployee);

                return addEmployee;

            }
            return null;
        }

        public async Task AddRangeEmployees(NewEmployee[] newEmployees)
        {
            var newEmployeeEntities = newEmployees.Select(employee =>
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
            await _employeeRepository.AddRangeAsync(newEmployeeEntities);
        }

        public async Task<Employee> UpdateEmployee(UpdateEmployeeViewModel employee)
        {
            var updateEmployee = await _employeeRepository.ByIdAsync(employee.EmployeeId);
            if (updateEmployee != null)
            {
                updateEmployee.Email = employee.Email;
                updateEmployee.FirstName = employee.FirstName;
                updateEmployee.LastName = employee.LastName;
                updateEmployee.Salary = employee.Salary;
                updateEmployee.Department = employee.Department;
                return await _employeeRepository.UpdateAsync(updateEmployee);
            }
            return updateEmployee;
        }

        public async Task<List<EmployeeNotFoundViewModel>> UpdateEmployees(UpdateEmployeeViewModel[] employees)
        {
            var employeeIdsNotFound = new List<EmployeeNotFoundViewModel>();
            var existingEmployees = await _employeeRepository.GetByIdsAsync(employees.Select(u => u.EmployeeId).ToList());

            var updatedEmployees = employees.Select(employee =>
            {
                var existingEmployee = existingEmployees.FirstOrDefault(u => u.EmployeeId == employee.EmployeeId);
                if (existingEmployee != null)
                {
                    existingEmployee.Email = employee.Email;
                    existingEmployee.FirstName = employee.FirstName;
                    existingEmployee.LastName = employee.LastName;
                    existingEmployee.Department = employee.Department;
                    existingEmployee.Status = employee.Status;
                    existingEmployee.Salary = employee.Salary;
                }
                return existingEmployee;
            }).Where(user => user != null).ToArray();


            if (updatedEmployees.Length > 0)
            {
                await _employeeRepository.UpdateRangeAsync(updatedEmployees);
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

        public async Task<bool> DeleteEmployee(int id)
        {
            Employee employee = await _employeeRepository.ByIdAsync(id);
            if (employee != null)
            {
                return await _employeeRepository.RemoveAsync(employee);
            }
            return false;
        }
        //-------    Sql queries on Employee table    -------

        public async Task<List<DepartmentViewModel>> EmployeeByDept()
        {
            return await _employeeRepository.EmpByDepartment();
        }

        public async Task<List<EmployeeViewModel>> EmployeeFromHR()
        {
            return await _employeeRepository.EmployeeFromHR();
        }

        public async Task<string> GerAllHireDates()
        {
            return await _employeeRepository.GetHiringDates();
        }

        public async Task<string> GetEmpFirstName()
        {
            return await _employeeRepository.GetAllFirstName();
        }

    }
}
