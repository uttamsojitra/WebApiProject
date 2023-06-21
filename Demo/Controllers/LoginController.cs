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

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            IActionResult response = Unauthorized();
            User validUser = await AuthnticateUser(email, password);
            if(validUser == null)
            {
                return response;
            }
            if (validUser.Status == false)
            {
                return Ok("Please activate your account");
            }
            if (validUser != null)
            {
                var (accessToken, refreshToken) = _authService.GenerateTokens(validUser);
                response = Ok(new { token = accessToken, refreshToken = refreshToken });
            }
            return response;
        }

        [AllowAnonymous]
        [HttpPost("RefreshTokenValidation")]
        public async Task<IActionResult> RefreshToken(string refreshToken)
        {
            var principal = _authService.ValidateRefreshToken(refreshToken);
            if (principal != null)
            {
                var email = principal.Claims.First(c => c.Type == "Email").Value;
                var user = await _authService.GetUserFromEmail(email);
                var (accessToken, newRefreshToken) = _authService.GenerateTokens(user);
                return Ok(new { token = accessToken, refreshToken = newRefreshToken });
            }
            return Unauthorized();
        }
    }
}
