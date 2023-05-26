using Demo.Entities.Data;
using Demo.Entities.Model;
using Demo.Repository.Interface;
using Demo.Repository.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Demo.Business.Exception;
using Demo.Business.Interface.Interface_Service;
using Microsoft.AspNetCore.Authorization;

namespace Demo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
       
        public UserController(IUserService userService)
        {
            _userService = userService;
        }
 
        [HttpGet]
        [Route("GetUserById")]
        public async Task<ActionResult<User>> GetUserById(int id)
        {
            var user = await _userService.GetUserById(id);
            if (user == null)
            {
                throw new ArgumentException(MessageHelper.InvalidUser);
            }
            return user;
        }

        [HttpGet]
        [Route("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers(int pageNumber = 1, int pageSize = 5)
        {
            var result = await _userService.GetAllUsers(pageNumber, pageSize);
            var totalUsers = _userService.GetTotalUsersCount();
            var totalPages = _userService.GetTotalPages(pageSize);

            var response = new
            {
                Items = result,
                TotalPages = totalPages,
                CurrentPage = pageNumber,
                TotalUsers = totalUsers
            };

            return Ok(response);
        }

        [HttpPost]
        [Route("CreateUser")]
        public async Task<string> CreateUser(User user)
        {
            await _userService.CreateUser(user);
            return "User Added";
        }

        [HttpPut]
        [Route("UpdateUser")]
        public async Task<IActionResult> UpdateUser(User user)
        {
            if (user == null)
            {
                return BadRequest("User not updated");
            }

            await _userService.UpdateUser(user);
            return Ok("User Updated");
        }

        [HttpDelete]
        [Route("DeleteUser")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var result = await _userService.DeleteUser(id);
            if (!result)
            {
                throw new InvalidOperationException(MessageHelper.UserNotRemove);
            }

            return NoContent();
        }
    }
}
