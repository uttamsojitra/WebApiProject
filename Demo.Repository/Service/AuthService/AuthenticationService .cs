using Demo.Business.Interface.Interface_Service;
using Demo.Entities.Model;
using Demo.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Demo.Business.Service.AuthService;

namespace Demo.Business.Service
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _config;

        public AuthenticationService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _config = configuration;
        }

        public async Task<User> GetAuthUser(string Email, string Password)
        {
            var user = await _userRepository.GetAuthUser(Email, Password);
            return user;
        }

        public async Task<User> GetUserFromEmail(string Email)
        {
            var user = await _userRepository.UserFromEmail(Email);
            return user;
        }
        public (string, string) GenerateTokens(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
            {
                new Claim("Email", user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            if (user.Role == "admin")
            {
                claims.Add(new Claim("permission", Permissions.CanUserCreate));
                claims.Add(new Claim("permission", Permissions.CanUserUpdate));
                claims.Add(new Claim("permission", Permissions.CanUserDelete));
            }
            else if (user.Role == "user")
            {
                claims.Add(new Claim("permission", Permissions.CanUserCreate));
                claims.Add(new Claim("permission", Permissions.CanUserUpdate));

                //claims.Add(new Claim("CanCreate", "True"));
                //claims.Add(new Claim("CanUpdate", "True"));
            }
            var accessToken = new JwtSecurityToken(
                _config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(5), // Set the access token expiration time
                signingCredentials: credentials);

            var refreshToken = new JwtSecurityToken(
                _config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddDays(2), // Set the refresh token expiration time
                signingCredentials: credentials);

            var accessTokenString = new JwtSecurityTokenHandler().WriteToken(accessToken);
            var refreshTokenString = new JwtSecurityTokenHandler().WriteToken(refreshToken);

            return (accessTokenString, refreshTokenString);
        }

        public ClaimsPrincipal ValidateRefreshToken(string token)
        {
            //handling and validating (JWTs).
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                //converting the specified key from the configuration to a byte array using UTF-8 encoding.
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"])),
                ValidateIssuer = true,
                ValidIssuer = _config["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = _config["Jwt:Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            //It is used to capture the validated security token if the token validation is successful.
            SecurityToken securityToken;
            ClaimsPrincipal principal = null;

            try
            {
                //ValidateToken method of the JwtSecurityTokenHandler
                var result = tokenHandler.ValidateToken(token, validationParameters, out securityToken);
                if (result.Identity is ClaimsIdentity claimsIdentity)
                {
                    principal = new ClaimsPrincipal(claimsIdentity);
                }
            }
            catch
            {
                return null;
            }
            return principal;
        }
    }
}
