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
using Demo.Entities.Models;

namespace Demo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAuthHelperService _authHelperService;


        public UserController(IUserService userService, IAuthHelperService authHelperService)
        {
            _userService = userService;
            _authHelperService = authHelperService; 
        }

        [AllowAnonymous]
        [HttpPost("signUpUser")]

        public async Task<IActionResult> CreateUser([FromForm] UserSignUpViewModel user)
        {
            var newUser = await _userService.CreateUser(user);
            if (newUser == null)
            {
                return Ok("Email Already Exists!");
            }
            return Ok("User Added Sucessfully!");
        }


        [HttpPost("addUsers")]
        [Authorize(Policy = "AddUpdatePolicy")]
        public async Task<IActionResult> AddUsers(UserSignUpViewModel[] users)
        {
            await _userService.AddRangeUsers(users);
            return Ok("Users added successfully.");
        }

        [HttpPut("updateUser")]
        [Authorize(Policy = "AddUpdatePolicy")]
        public async Task<IActionResult> UpdateUser(UpdateUserViewModel user)
        {
            var updatedUser = await _userService.UpdateUser(user);
            if (updatedUser != null)
            {
                return Ok("User updated successfully");
            }
            else
            {
                return BadRequest("User not found");
            }

        }

        [HttpPut("updateRangeUsers")]
        public async Task<ActionResult<List<EmployeeNotFoundViewModel>>> UpdateUsers(UpdateUserViewModel[] users)
        {
            var userIdsNotFound = await _userService.UpdateUsers(users);

            if (userIdsNotFound.Count > 0)
            {
                var errorMessage = "The following users not found, please enter correct UserId ";
                return BadRequest(new { Message = errorMessage, userIdsNotFound });
            }

            return Ok("All Users Updated ");
        }

        [HttpDelete("deleteUser")]
        [Authorize(Policy = "DeletePolicy")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            //var permission = _authHelperService.Permissions.Any(p => p.Item1 == "CanDelete" && p.Item2);
            //{
            //    throw new Exception("Insufficient permissions to delete user.");
            //}
            var result = await _userService.DeleteUser(id);
            if (!result)
            {
                throw new InvalidOperationException(MessageHelper.UserNotRemove);
            }
           
            return Ok("User Deleted");
        }

        [HttpGet("users/{id}")]
        public async Task<ActionResult<User>> GetUserById(int id)
        {
            var user = await _userService.GetUserById(id);
            if (user == null)
            {
                throw new ArgumentException(MessageHelper.InvalidUser);
            }
            return user;
        }

        [HttpGet("getAllUsers")]
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

        [HttpGet("getAllSkills")]
        public async Task<IActionResult> GetSkillList()
        {
            var result = await _userService.GetAllSkills();
            return Ok(result);
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

        //----   Export to Excel-Word-Pdf
        [HttpGet("userExcelFile")]
        public async Task<IActionResult> ExportUsersToExcel()
        {
            var excelBytes = await _userService.ExportUsersDataToExcel();
            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "users.xlsx");
        }

        [HttpGet("userPdfFile")]
        public async Task<IActionResult> ExportUsersToPDF()
        {
            var pdfBytes = await _userService.ExportUsersDataToPDF();
            var fileName = "users.pdf"; // Specify the desired file name
            return File(pdfBytes, "application/pdf", fileName);
        }

        [HttpGet("userWordFile")]
        public async Task<IActionResult> ExportUsersToWord()
        {
            var wordBytes = await _userService.ExportUsersDataToWord();
            return File(wordBytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "users.docx");
        }

        //-- Upload excelfile and store data in Database
        [HttpPost("uploadFile")]
        public async Task<IActionResult> UploadUsers(IFormFile file)
        {
            if (file == null || file.Length <= 0)
            {
                return BadRequest("No file uploaded.");
            }

            using var fileStream = file.OpenReadStream();

            var responseModel = await _userService.StoreUsersFromExcel(fileStream);

            if (responseModel.UsersWithNullEmail.Any())
            {
                var errorMessage = "The following users have null email: ";
                return BadRequest(new { Message = errorMessage, responseModel.UsersWithNullEmail });
            }

            return Ok("Users uploaded and stored in the database.");
        }

        //var hasAdminPermission = User.Claims.Any(c => c.Type == "permission" && c.Value == "delete");
        //var permission = _authHelperService.Permissions.Any(p => p.Item1 == "CanDelete" && p.Item2);

        //{
        //    throw new Exception("Insufficient permissions to delete user.");
        //}
    }
}
