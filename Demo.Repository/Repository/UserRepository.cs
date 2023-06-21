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


        public UserRepository(UserDbcontext userDbContext, CiPlatformContext ciPlatformContext) : base(userDbContext)
        {
            _userDbContext = userDbContext;           
            _ciPlatformContext = ciPlatformContext;
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

        public async Task<User> GetUserEmail(string email)
        {
            return await _userDbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        }
       
        public async Task<List<User>> GetByIdsAsync(List<long> userIds)
        {
            return await _userDbContext.Users.Where(u => userIds.Contains(u.UserId)).ToListAsync();
        }

        // for export file data
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

        public async Task<User> GetAuthUser(string Email, string Password)
        {
            User user = await _userDbContext.Users.FirstOrDefaultAsync(u => u.Email == Email && u.Password == Password);
            return user;
        }
        public async Task<User> UserFromEmail(string Email)
        {
            User user = await _userDbContext.Users.FirstOrDefaultAsync(u => u.Email == Email);
            return user;
        }

        //Add New User and Mail sent for status activation

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

    }
}


