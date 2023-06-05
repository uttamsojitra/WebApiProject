using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Demo.Business.Interface;
using Demo.Entities.Data;
using Demo.Entities.Model;
using Demo.Entities.Model.ViewModel;
using Demo.Repository.Interface;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Demo.Repository.Repository
{
    public class UserRepository : IUserRepository
    {
       
        private readonly UserDbcontext _userDbContext;
        private readonly string _connectionString;
        private readonly IEmailSender _emailSender;

        public UserRepository(UserDbcontext userDbContext, IEmailSender emailSender)
        {
            _userDbContext = userDbContext;
            _connectionString = GetConnectionString();
            _emailSender = emailSender;
        }

        private string GetConnectionString()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            return configuration.GetConnectionString("DefaultConnection");
        }
        //------    DataTable stores result of query     -----
        private async Task<DataTable> ExecuteQuery(string sqlQuery)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                SqlDataAdapter adapter = new SqlDataAdapter(sqlQuery, connection);
                DataTable table = new DataTable();
                adapter.Fill(table);
                return table;
            }
        }

        //------   User CRUD operation    -----
        public async Task<List<User>> GetUserList()
        {
            string sqlQuery = "SELECT UserId, FirstName, LastName, STUFF(Email, 1, 3, '***') AS MaskedEmail, PhoneNumber, Password FROM Users";
            DataTable table = await ExecuteQuery(sqlQuery);

            List<User> users = table.Rows.Cast<DataRow>().Select(row => new User
            {
                UserId = Convert.ToInt32(row["UserId"]),
                FirstName = row["FirstName"].ToString(),
                LastName = row["LastName"].ToString(),
                Email = row["MaskedEmail"].ToString(),
                Password = row["Password"].ToString(),
                PhoneNumber = Convert.ToInt64(row["PhoneNumber"])
            }).ToList();

            return users;
        }

        public async Task<User> GetUser(long id)
        {
            var user = await _userDbContext.Users.FindAsync(id);
            return user;
        }
       
        public async Task<User> GetAuthUser(string Email, string Password)
        {
            User user = await _userDbContext.Users.FirstOrDefaultAsync(u => u.Email == Email && u.Password == Password);
            return user;
        }


        //Add New User and Mail sent for status activation
        private string CreateRandomToken()
        {
            return Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
        }
        public async Task<User> AddUser(UserSignUpViewModel user)
        {
           var ExistUser = await _userDbContext.Users.FirstOrDefaultAsync(u => u.Email == user.Email);

            if (ExistUser == null)
            { 
                var token = CreateRandomToken();

                User newUser = new();
               
                newUser.Email = user.Email;
                newUser.Password = user.Password;
                newUser.FirstName = user.FirstName; 
                newUser.LastName = user.LastName;   
                newUser.PhoneNumber = user.PhoneNumber;
                newUser.Status = false;
                newUser.Token = token;
                
                _userDbContext.Users.Add(newUser);
                await _userDbContext.SaveChangesAsync();

                var activationLink = "https://localhost:7149/api/User/activate?email=" + user.Email + "&token=" + token;

                var message = "Please active your account by clicking link " + activationLink ;
                
                var subject = "User Status ActivationLink";
                await _emailSender.SendEmailAsync(user.Email, message, subject);

                return newUser;
            }
            return null;   
        }

        public async Task<User> GetUserByEmailAndToken(string email, string token)
        {
            var user = await _userDbContext.Users.FirstOrDefaultAsync(u => u.Email == email && u.Token == token);
            if (user != null)
            {
                user.Status = true;
                _userDbContext.Update(user);
                _userDbContext.SaveChangesAsync();
                return user;
            }
            return user;
        }
        public async Task<User> GetUserStatus(string email, string token)
        {
            var user = await _userDbContext.Users.FirstOrDefaultAsync(u => u.Email == email && u.Token == token);
            return user;
        }

        public async Task UpdateUser(User user)
        {
            var existingUser = await _userDbContext.Users.FirstOrDefaultAsync(u => u.UserId == user.UserId);
            if (existingUser == null)
            {
                throw new ArgumentException("User not found");
            }
            existingUser.FirstName = user.FirstName;
            existingUser.LastName = user.LastName;
            existingUser.Email = user.Email;
            existingUser.PhoneNumber = user.PhoneNumber;
            await _userDbContext.SaveChangesAsync();
        }

        public async Task<bool> RemoveUser(long id)
        {
            var user = await _userDbContext.Users.FindAsync(id);
            if (user != null)
            {
                _userDbContext.Users.Remove(user);
                await _userDbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public int GetTotalUsersCount()
        {
            return _userDbContext.Users.Count();
        }

        //-- Pivot Query  ---
       public async Task<List<DepartmentViewModel>> EmpByDepartment()
        {
            string sqlQuery = "SELECT * FROM(SELECT EmployeeId, CONCAT(FirstName, ' ', LastName) AS FullName, Department FROM employees) AS SourceTable  PIVOT(  COUNT(employeeId) FOR Department  IN([Sales], [IT],[HR]) ) AS pivot_table ";
            DataTable table = await ExecuteQuery(sqlQuery);

            List<DepartmentViewModel> departments = table.Rows.Cast<DataRow>().Select(row => new DepartmentViewModel
            {
                FullName = row["FullName"].ToString(),
                IT = row["IT"].ToString(),
                Sales = row["Sales"].ToString(),
                HR = row["HR"].ToString()
            }).ToList();

            return departments;
        }

        //----- CTE Query ------
        public async Task<List<EmployeeViewModel>> EmployeeFromHR()
        {
            string sqlQuery = "WITH HR_Employees AS(SELECT EmployeeId, CONCAT(FirstName, ' ', LastName) AS FullName, CONVERT(varchar, HireAt, 3) AS FormattedDate, DATEPART(yyyy, HireAt) AS HireYear FROM Employees WHERE Department = 'HR') SELECT * FROM HR_Employees;";
            DataTable table = await ExecuteQuery(sqlQuery);

            List<EmployeeViewModel> employees = table.Rows.Cast<DataRow>().Select(row => new EmployeeViewModel
            {
                EmployeeId = Convert.ToInt32(row["EmployeeId"]),
                FullName = row["FullName"].ToString(),
                HireYear = Convert.ToInt32(row["HireYear"]),
                HireAt = row["FormattedDate"].ToString()
            }).ToList();

            return employees;
        }
       
        //----- xml path query ------
        public async Task<string> GetHiringDates()
        {
            string sqlQuery = "SELECT STUFF((SELECT ',' + CONVERT(VARCHAR(10), HireAt, 120) FROM Employees FOR XML PATH(''), TYPE).value('.', 'VARCHAR(MAX)'), 1, 1, '') AS HiringDates";
            DataTable table = await ExecuteQuery(sqlQuery);

            string hiringDates = table.Rows.Count > 0 ? table.Rows[0]["HiringDates"].ToString() : string.Empty;
            return hiringDates;
        }

        //-----   xml path alternatives  - STRING_AGG  ------
        public async Task<string> GetAllFirstName()
        {
            string sqlQuery = "SELECT STRING_AGG(FirstName, ', ') AS ConcatenatedNames FROM Employees;";
            DataTable table = await ExecuteQuery(sqlQuery);

            string firstNames = table.Rows.Count > 0 ? table.Rows[0]["ConcatenatedNames"].ToString() : string.Empty;
            return firstNames;
        }

        
    }
}


