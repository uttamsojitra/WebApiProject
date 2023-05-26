using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demo.Entities.Data;
using Demo.Entities.Model;
using Demo.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace Demo.Repository.Repository
{
    public class UserRepository : IUserRepository
    {
        public readonly UserDbcontext _UserDbContext;

        public UserRepository(UserDbcontext userContext)
        {
            _UserDbContext = userContext;
        }

        public async Task<List<User>> GetUserList()
        {
            List<User> users = await _UserDbContext.Users.ToListAsync();
            return users;
        }

        public async Task<User> GetUser(long id)
        {
            var user = await _UserDbContext.Users.FindAsync(id);
            return user;
        }

        public async Task<User> GetAuthUser(string FirstName, string Password)
        {
            User user = await _UserDbContext.Users.FirstOrDefaultAsync(u => u.FirstName == FirstName && u.Password == Password);
            return user;
        }
        public async Task AddUser(User user)
        {
            _UserDbContext.Users.Add(user);
            await _UserDbContext.SaveChangesAsync();
        }

        public async Task UpdateUser(User user)
        {
            var existingUser = await _UserDbContext.Users.FirstOrDefaultAsync(u => u.UserId == user.UserId);
            if (existingUser == null)
            {
                throw new ArgumentException("User not found");
            }

            existingUser.FirstName = user.FirstName;
            existingUser.LastName = user.LastName;
            existingUser.Email = user.Email;
            existingUser.PhoneNumber = user.PhoneNumber;

            await _UserDbContext.SaveChangesAsync();
        }

        public async Task<bool> RemoveUser(long id)
        {
            var user = await _UserDbContext.Users.FindAsync(id);
            if (user != null)
            {
                _UserDbContext.Users.Remove(user);
                await _UserDbContext.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public int GetTotalUsersCount()
        {
            return _UserDbContext.Users.Count();
        }

        public async Task<User> GetUserByUsername(string username)
        {
            // Implement the logic to retrieve a user from the database based on the provided username
            var user = await _UserDbContext.Users.FirstOrDefaultAsync(u => u.FirstName == username);
            return user;
        }
    }
}
