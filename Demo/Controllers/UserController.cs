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

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("SignUpUser")]
        public async Task<IActionResult> CreateUser([FromForm] UserSignUpViewModel user)
        {
            var newUser = await _userService.CreateUser(user);
            if (newUser == null)
            {
                return Ok("Email Already Exists!");
            }
            return Ok("User Added Sucessfully!");
        }

        [HttpPost("AddUsers")]
        public async Task<IActionResult> AddUsers(UserSignUpViewModel[] users)
        {
            try
            {
                await _userService.AddRangeUsers(users);
                return Ok("Users added successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPut]
        [Route("UpdateUser")]
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

        [HttpPut("UpdateRangeUsers")]
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

        [HttpGet]
        [Route("GetAllSkills")]
        public async Task<IActionResult> GetSkillList()
        {
            var result = await _userService.GetAllSkills();
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("Activate")]
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


        //----   SQL - Queries 
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


        //----   Export to Excel-Word-Pdf
        [HttpGet("UserExcelFile")]
        public async Task<IActionResult> ExportUsersToExcel()
        {
            var excelBytes = await _userService.ExportUsersDataToExcel();
            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "users.xlsx");
        }

        [HttpGet("UserPdfFile")]
        public async Task<IActionResult> ExportUsersToPDF()
        {
            var pdfBytes = await _userService.ExportUsersDataToPDF();
            var fileName = "users.pdf"; // Specify the desired file name
            return File(pdfBytes, "application/pdf", fileName);
        }

        [HttpGet("UserWordFile")]
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

        [HttpGet("GetAllEmployeesName")]
        public async Task<IActionResult> GetEmployeeName()
        {
            var Emplyee = await _userService.GetEmployeesName();
            return Ok(Emplyee);
        }

        [HttpGet("employee/count-by-department")]
        public async Task<ActionResult<Dictionary<string, int>>> GetEmployeeCountByDepartment()
        {
            var countByDepartment = await _userService.GetEmployeeCountByDepartment();
            return Ok(countByDepartment);
        }


        //--- CRUD - Employee  ----

        [AllowAnonymous]
        [HttpPost]
        [Route("AddEmployee")]
        public async Task<IActionResult> CreateEmployee(NewEmployee employee)
        {
            var newUser = await _userService.CreateEmployee(employee);
            if (newUser == null)
            {
                return Ok("Email Already Exists!");
            }
            return Ok("Employee Added Sucessfully!");
        }

        [HttpPost("AddEmployees")]
        public async Task<IActionResult> AddEmployees(NewEmployee[] employees)
        {
            try
            {
                await _userService.AddRangeEmployees(employees);
                return Ok("Employees added successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.InnerException.Message}");
            }
        }

        [HttpGet]
        [Route("GetEmployeeById")]
        public async Task<ActionResult<Employee>> GetEmployeeById(int id)
        {
            var employee = await _userService.GetEmployeeById(id);
            if (employee == null)
            {
                throw new ArgumentException(MessageHelper.InvalidUser);
            }
            return employee;
        }

        [HttpDelete]
        [Route("DeleteEmployee")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var result = await _userService.DeleteEmployee(id);
            if (!result)
            {
                throw new InvalidOperationException(MessageHelper.UserNotRemove);
            }
            return Ok("Employee Deleted");
        }

        [HttpPut]
        [Route("UpdateEmployee")]
        public async Task<IActionResult> UpdateEmployee(UpdateEmployeeViewModel employee)
        {
            var updatedUser = await _userService.UpdateEmployee(employee);
            if (updatedUser != null)
            {
                return Ok("Employee updated successfully");
            }
            else
            {
                return BadRequest("Employee not found");
            }
        }

        [HttpPut("UpdateRangeEmployees")]
        public async Task<ActionResult<List<EmployeeNotFoundViewModel>>> UpdateEmployees(UpdateEmployeeViewModel[] users)
        {
            var employeeIdsNotFound = await _userService.UpdateEmployees(users);

            if (employeeIdsNotFound.Count > 0)
            {
                var errorMessage = "The following employees not found, please enter correct EmployeeId ";
                return BadRequest(new { Message = errorMessage, employeeIdsNotFound });
            }

            return Ok("All Users Updated ");
        }
    }
}
