using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Demo.Business.Interface.Interface_Service;

namespace Demo.Business.Service
{
    public class AuthHelperService : IAuthHelperService
    {

        private readonly IHttpContextAccessor _httpContextAccessor;
       
        public AuthHelperService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor; 
            
        }

        private JwtSecurityToken SecurityToken
        {
            get
            {
                string authHeader  = _httpContextAccessor.HttpContext?.Request?.Headers["Authorization"].FirstOrDefault();  
                if (authHeader != null)
                {
                    string token = authHeader.ToString().Split(' ')?[1] ?? string.Empty;
                    JwtSecurityTokenHandler handler = new();
                    return handler.ReadToken(token) as JwtSecurityToken;
                }
                return null;    
            }
        }

        public int UserId
        {
            get
            {
                if (SecurityToken != null)
                {
                    string userIdClaim = SecurityToken.Claims.FirstOrDefault(x => x.Type == "UserId")?.Value;
                    if (int.TryParse(userIdClaim, out int userId))
                    {
                        return userId;
                    }
                }
                return 0; // or any default value you prefer when the UserId cannot be retrieved
            }
        }

        public List<Tuple<string, bool>> Permissions
        {
            get
            {
                if (SecurityToken == null)
                {
                    return new();
                }
                List<Tuple<string, bool>> Permissions = new();
                
                SecurityToken.Claims.Where(x => x.Type == "permission").ToList().ForEach(claim =>
                {
                    Permissions.Add(new Tuple<string, bool>(claim.Value, true));
                });
                return Permissions;

            }
        }
    }
}
