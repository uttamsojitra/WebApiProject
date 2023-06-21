using Dapper;
using Demo.Business.Interface;
using Demo.Entities.Data;
using Demo.Entities.Model;
using Demo.Entities.Model.ViewModel;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Business.Repository
{
    public class EmployeeRepository : BaseRepository<Employee>, IEmployeeRepository
    {
        private readonly UserDbcontext _userDbContext;
        private readonly string _connectionString;

        public EmployeeRepository(UserDbcontext userDbcontext) : base(userDbcontext)
        {
            _userDbContext = userDbcontext;
            _connectionString = GetConnectionString();
        }

        private static string GetConnectionString()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            return configuration.GetConnectionString("DefaultConnection");
        }
        public async Task<List<string>> GetEmployeeByName()
        {
            return await _userDbContext.Employees.Select(emp => emp.FirstName +" "+emp.LastName).ToListAsync();         
        }

        public async Task<List<Employee>> GetEmployeesAsync()
        {
            return await _userDbContext.Employees.ToListAsync();
        }
       
        public async Task<Employee> GetEmployeeEmail(string email)
        {
            return await _userDbContext.Employees.FirstOrDefaultAsync(u => u.Email == email);
        }
       
        public async Task<List<Employee>> GetByIdsAsync(List<long> employeeIds)
        {
            return await _userDbContext.Employees.Where(u => employeeIds.Contains(u.EmployeeId)).ToListAsync();
        }

        //-- Pivot Query  ---
        public async Task<List<DepartmentViewModel>> EmpByDepartment()
        {
            string sqlQuery = "SELECT * FROM(SELECT EmployeeId, CONCAT(FirstName, ' ', LastName) AS FullName, Department FROM employees) AS SourceTable  PIVOT(  COUNT(employeeId) FOR Department  IN([Sales], [IT],[HR]) ) AS pivot_table ";

            using IDbConnection connection = new SqlConnection(_connectionString);

            var result = await connection.QueryAsync<DepartmentViewModel>(sqlQuery);
            List<DepartmentViewModel> departments = result.ToList();
            return departments;
        }

        //----- CTE Query ------
        public async Task<List<EmployeeViewModel>> EmployeeFromHR()
        {
            string sqlQuery = "WITH HR_Employees AS(SELECT EmployeeId, CONCAT(FirstName, ' ', LastName) AS FullName, CONVERT(varchar, HireAt, 3) AS HireAt, DATEPART(yyyy, HireAt) AS HireYear FROM Employees WHERE Department = 'HR') SELECT * FROM HR_Employees;";

            using IDbConnection connection = new SqlConnection(_connectionString);

            var result = await connection.QueryAsync<EmployeeViewModel>(sqlQuery);
            List<EmployeeViewModel> employees = result.ToList();
            return employees;
        }

        //----- xml path query ------
        public async Task<string> GetHiringDates()
        {
            string sqlQuery = "SELECT STUFF((SELECT ',' + CONVERT(VARCHAR(10), HireAt, 120) FROM Employees FOR XML PATH(''), TYPE).value('.', 'VARCHAR(MAX)'), 1, 1, '') AS HiringDates;";

            using IDbConnection connection = new SqlConnection(_connectionString);

            var result = await connection.QueryFirstOrDefaultAsync<string>(sqlQuery);
            string hiringDates = result ?? string.Empty;
            return hiringDates;
        }

        //----- xml path alternatives  - STRING_AGG  ------
        public async Task<string> GetAllFirstName()
        {
            string sqlQuery = "SELECT STRING_AGG(FirstName, ', ') AS ConcatenatedNames FROM Employees;";

            using IDbConnection connection = new SqlConnection(_connectionString);

            var result = await connection.QueryFirstOrDefaultAsync<string>(sqlQuery);
            string firstNames = result ?? string.Empty;
            return firstNames;
        }
    }
}
