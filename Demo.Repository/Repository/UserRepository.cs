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
using Demo.Entities.Models;
using Demo.Repository.Interface;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System.Text.Json.Serialization;
using Dapper;
using Demo.Business.Repository;

namespace Demo.Repository.Repository
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        private readonly UserDbcontext _userDbContext;
        private readonly CiPlatformContext _ciPlatformContext;
        private readonly string _connectionString;
        private readonly IEmailSender _emailSender;

        public UserRepository(UserDbcontext userDbContext, IEmailSender emailSender, CiPlatformContext ciPlatformContext) : base(userDbContext)
        {
            _userDbContext = userDbContext;
            _connectionString = GetConnectionString();
            _emailSender = emailSender;
            _ciPlatformContext = ciPlatformContext;
        }

        private static string GetConnectionString()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            return configuration.GetConnectionString("DefaultConnection");
        }

        //------    DataTable stores result of query     -----
        //private async Task<DataTable> ExecuteQuery(string sqlQuery)
        //{
        //    using (SqlConnection connection = new SqlConnection(_connectionString))
        //    {
        //        await connection.OpenAsync();

        //        SqlDataAdapter adapter = new SqlDataAdapter(sqlQuery, connection);
        //        DataTable table = new();
        //        adapter.Fill(table);
        //        return table;
        //    }
        //}

        //------   User CRUD operation    -----

        public async Task<User> GetUser(long id)
        {
            return await ByIdAsync(id);
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

                await AddAsync(newUser);

                string templatePath = Path.Combine("Template", "Account_Activation_EmailTemplate.html");
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), templatePath);

                string MailText;
                using (StreamReader reader = new(filePath))
                {
                    MailText = reader.ReadToEnd();
                }

                string activationLink = "https://localhost:7149/api/User/activate?email=" + user.Email + "&token=" + token;

                string emailContent = MailText
                    .Replace("{{UserName}}", newUser.FirstName + " " + newUser.LastName)
                    .Replace("{{ActivationLink}}", activationLink);


                var subject = "User Status ActivationLink";
                await _emailSender.SendEmailAsync(user.Email, emailContent, subject);

                return newUser;

            }
            return null;
        }

        public async Task AddUsers(UserSignUpViewModel[] users)
        {
            var newUserEntities = users.Select(user =>
            {
                return new User
                {
                    Email = user.Email,
                    Password = user.Password,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber,
                    Status = false,
                    Token = CreateRandomToken()
                };
            }).ToArray();
            await AddRangeAsync(newUserEntities);
        }

        public async Task<User> UpdateUser(UpdateUserViewModel user)
        {
            User updateUser = await ByIdAsync(user.UserId);
            if (updateUser != null)
            {
                updateUser.Email = user.Email;
                updateUser.FirstName = user.FirstName;
                updateUser.LastName = user.LastName;
                updateUser.PhoneNumber = user.PhoneNumber;
                updateUser.Status = user.Status;
                updateUser.Password = user.Password;

                return await UpdateAsync(updateUser);
            }
            return updateUser;
        }

        public async Task<List<UserNotFoundViewModel>> UpdateUsers(UpdateUserViewModel[] users)
        {
            var userIdsNotFound = new List<UserNotFoundViewModel>();
            var existingUsers = await GetByIdsAsync(users.Select(u => u.UserId).ToList());

            var updatedUsers = users.Select(user =>
            {
                var existingUser = existingUsers.FirstOrDefault(u => u.UserId == user.UserId);
                if (existingUser != null)
                {
                    existingUser.Email = user.Email;
                    existingUser.FirstName = user.FirstName;
                    existingUser.LastName = user.LastName;
                    existingUser.PhoneNumber = user.PhoneNumber;
                    existingUser.Status = user.Status;
                    existingUser.Password = user.Password;
                }
                return existingUser;
            }).Where(user => user != null).ToArray();


            if (updatedUsers.Length > 0)
            {
                await UpdateRangeAsync(updatedUsers);
            }

            userIdsNotFound = users.Where(user => updatedUsers.All(u => u.UserId != user.UserId))
                                  .Select(user => new UserNotFoundViewModel
                                  {
                                      UserId = user.UserId,
                                      UserName = user.FirstName + " " + user.LastName
                                  })
                                  .ToList();

            return userIdsNotFound;
        }


        public async Task<List<User>> GetByIdsAsync(List<long> userIds)
        {
            return await _userDbContext.Users.Where(u => userIds.Contains(u.UserId)).ToListAsync();
        }

        public async Task<bool> RemoveUser(long id)
        {
            User user = await ByIdAsync(id);
            if (user != null)
            {
                return await RemoveAsync(user);
            }
            return false;
        }

        public async Task<List<User>> GetUsersData()
        {
            return await _userDbContext.Users.ToListAsync();
        }

        public async Task<List<string>> GetSkills()
        {
            return await _ciPlatformContext.Skills.Select(a => a.SkillName).ToListAsync();
        }

        public async Task<List<User>> GetUserList()
        {
            string sqlQuery = "SELECT UserId, FirstName, LastName, STUFF(Email, 1, 3, '***') AS MaskedEmail, PhoneNumber, Password FROM Users";
            using IDbConnection connection = new SqlConnection(_connectionString);
            var result = await connection.QueryAsync<User>(sqlQuery);
            List<User> users = result.ToList();

            return users;
        }

        //public async Task<User> GetUser(long id)
        //{
        //    var user = await _userDbContext.Users.FindAsync(id);
        //    return user;
        //}

        //public async Task UpdateUser(User user)
        //{
        //    var existingUser = await _userDbContext.Users.FirstOrDefaultAsync(u => u.UserId == user.UserId);
        //    if (existingUser == null)
        //    {
        //        throw new ArgumentException("User not found");
        //    }
        //    existingUser.FirstName = user.FirstName;
        //    existingUser.LastName = user.LastName;
        //    existingUser.Email = user.Email;
        //    existingUser.PhoneNumber = user.PhoneNumber;
        //    await _userDbContext.SaveChangesAsync();
        //}

        //public async Task<bool> RemoveUser(long id)
        //{
        //    var user = await _userDbContext.Users.FindAsync(id);
        //    if (user != null)
        //    {
        //        _userDbContext.Users.Remove(user);
        //        await _userDbContext.SaveChangesAsync();
        //        return true;
        //    }
        //    return false;
        //}

        public async Task<User> GetAuthUser(string Email, string Password)
        {
            User user = await _userDbContext.Users.FirstOrDefaultAsync(u => u.Email == Email && u.Password == Password);
            return user;
        }

        //Add New User and Mail sent for status activation
        private static string CreateRandomToken()
        {
            return Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
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

        public int GetTotalUsersCount()
        {
            return _userDbContext.Users.Count();
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

        //public async Task<List<EmployeeViewModel>> EmployeeFromHR()
        //{
        //    string sqlQuery = "WITH HR_Employees AS(SELECT EmployeeId, CONCAT(FirstName, ' ', LastName) AS FullName, CONVERT(varchar, HireAt, 3) AS FormattedDate, DATEPART(yyyy, HireAt) AS HireYear FROM Employees WHERE Department = 'HR') SELECT * FROM HR_Employees;";
        //    DataTable table = await ExecuteQuery(sqlQuery);

        //    List<EmployeeViewModel> employees = table.Rows.Cast<DataRow>().Select(row => new EmployeeViewModel
        //    {
        //        EmployeeId = Convert.ToInt32(row["EmployeeId"]),
        //        FullName = row["FullName"].ToString(),
        //        HireYear = Convert.ToInt32(row["HireYear"]),
        //        HireAt = row["FormattedDate"].ToString()
        //    }).ToList();

        //    return employees;
        //}

        //----- xml path query ------
        public async Task<string> GetHiringDates()
        {
            string sqlQuery = "SELECT STUFF((SELECT ',' + CONVERT(VARCHAR(10), HireAt, 120) FROM Employees FOR XML PATH(''), TYPE).value('.', 'VARCHAR(MAX)'), 1, 1, '') AS HiringDates;";

            using IDbConnection connection = new SqlConnection(_connectionString);
            var result = await connection.QueryFirstOrDefaultAsync<string>(sqlQuery);

            string hiringDates = result ?? string.Empty;
            return hiringDates;
        }

        //-----   xml path alternatives  - STRING_AGG  ------
        public async Task<string> GetAllFirstName()
        {
            string sqlQuery = "SELECT STRING_AGG(FirstName, ', ') AS ConcatenatedNames FROM Employees;";

            using IDbConnection connection = new SqlConnection(_connectionString);
            var result = await connection.QueryFirstOrDefaultAsync<string>(sqlQuery);

            string firstNames = result ?? string.Empty;
            return firstNames;
        }

        public async Task AddUserFromFile(User user)
        {
            // Add the user entity to the database context
            _userDbContext.Users.Add(user);
            await _userDbContext.SaveChangesAsync();
        }

    }
}


