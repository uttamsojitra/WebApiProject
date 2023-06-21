using Demo.Entities.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Business.Interface.Interface_Service
{
    public interface IAuthenticationService
    {
        public Task<User> GetAuthUser(string Email, string Password);
        public (string, string) GenerateTokens(User user);
        public ClaimsPrincipal ValidateRefreshToken(string token);
        public Task<User> GetUserFromEmail(string Email);
    }

}
