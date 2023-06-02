using Demo.Business.Exception;
using Demo.Business.Interface.Interface_Service;
using Demo.Entities.Model;
using Demo.Repository.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Demo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IAuthenticationService _authService;
        public LoginController(IConfiguration configuration, IAuthenticationService authService)
        {
            _config = configuration;
            _authService = authService;        
        }

       private async Task<User> AuthnticateUser(string Email, string Password)
        {
            User user = await _authService.GetAuthUser(Email, Password);
            if(user == null )
            {
                return null;
            }
            return user;   
        }

        private (string, string) GenerateTokens(string Email)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
               new Claim("Email", Email)
             };

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


        private ClaimsPrincipal ValidateRefreshToken(string token)
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

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(string Email, string Password)
        {
            IActionResult response = Unauthorized();
            
            
            var validUser = await AuthnticateUser(Email, Password);
            if (validUser != null)
            {
                var (accessToken, refreshToken) = GenerateTokens(Email);
                response = Ok(new { token = accessToken, refreshToken = refreshToken });
            }
            return response;
        }

        [AllowAnonymous]
        [HttpPost("RefreshTokenValidation")]
        public IActionResult RefreshToken(string refreshToken)
        {
            var principal = ValidateRefreshToken(refreshToken);
            if (principal != null)
            {
                var email = principal.Claims.First(c => c.Type == "Email").Value;
                var (accessToken, newRefreshToken) = GenerateTokens(email);
                return Ok(new { token = accessToken, refreshToken = newRefreshToken });
            }
            return Unauthorized();
        }
    }
}
