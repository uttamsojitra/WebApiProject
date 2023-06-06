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
using Demo.Entities.Model.ViewModel;
using System.Security.Cryptography;

namespace Demo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
       
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("SignUpUser")]
        public async Task<IActionResult> CreateUser( [FromForm] UserSignUpViewModel user)
        {
           var newUser =  await _userService.CreateUser(user);
            if(newUser == null)
            {
                return Ok("Email Already Exists!");
            }
            return Ok("User Added Sucessfully!");         
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



        [AllowAnonymous]
        [HttpGet("activate")]
        public async Task<IActionResult> ActivateAccount(string email, string token)
        {
            // Find the user by email and activation token
            User userStatus = await _userService.GetUserStatus(email, token);
            if (userStatus == null)
            {
                return BadRequest("Invalid activation token or email.");
            }
            if (userStatus.Status == true)
            {
                return Ok("Status Already Activated");
            }

            User user = await _userService.GetEmailAndToken(email, token);
            if (user == null)
            {
                return BadRequest("Invalid activation token or email.");
            }
            return Ok("Your account has been activated successfully.");
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
            return Ok("User Deleted");
        }

        [HttpGet]
        [Route("GetEmployeesByDepartment")]
        public async Task<ActionResult<List<DepartmentViewModel>>> GetEmployeesByDepartment()
        {
            List<DepartmentViewModel> departments = await _userService.EmployeeByDept();
            return Ok(departments);
        }

        [HttpGet]
        [Route("GetEmployeesFromHR")]
        public async Task<ActionResult<List<EmployeeViewModel>>> GetEmployeesFromHR()
        {
            List<EmployeeViewModel> employees = await _userService.EmployeeFromHR();
            return Ok(employees);
        }

        [HttpGet]
        [Route("GerAllHireDates")]
        public async Task<ActionResult<string>> GerAllHireDates()
        {
            string dates = await _userService.GerAllHireDates();
            return Ok(dates);
        }

        [HttpGet]
        [Route("GetEmployeesFirstName")]
        public async Task<ActionResult<string>> GetEmployeesFirstName()
        {
            string names = await _userService.GetEmpFirstName();
            return Ok(names);
        }

        [HttpGet("exportUser")]
        public async Task<IActionResult> ExportUsersToExcel()
        {
            var excelBytes = await _userService.ExportUsersDataToExcel();

            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "users.xlsx");
        }
    }
}
