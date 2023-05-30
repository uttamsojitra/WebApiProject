using Demo.Business.Interface.Interface_Service;
using Demo.Entities.Model;
using Demo.Repository.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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

        private string GenerateToken(string Email, string Password)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim("Email",Email),
                new Claim("Password",Password),
            };
            var token = new JwtSecurityToken(
                _config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: credentials);
       
            return new JwtSecurityTokenHandler().WriteToken(token);
            //Uses an instance of JwtSecurityTokenHandler to write the JWT token as a string.
        }   
        
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(string Email, string Password)
        {
            IActionResult response = Unauthorized();
            var validUser = await AuthnticateUser(Email, Password);
            if (validUser != null)
            {
                var token = GenerateToken(Email, Password);
                HttpContext.Response.Cookies.Append("token", token, new CookieOptions { HttpOnly = true, Secure = true, SameSite = SameSiteMode.None, Expires = DateTime.Now.AddMinutes(5) });
                response =  Ok(new {token = token});                 
            }
            return response;        
        }
    }
}
