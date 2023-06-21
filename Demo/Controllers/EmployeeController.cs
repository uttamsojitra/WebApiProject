using Demo.Business.Exception;
using Demo.Business.Interface.Interface_Service;
using Demo.Entities.Model;
using Demo.Entities.Model.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UserDemo.Data.migrations;

namespace Demo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        //--- CRUD - Employee  ----

        [AllowAnonymous]
        [HttpPost]
        [Route("addEmployee")]
        public async Task<IActionResult> CreateEmployee(NewEmployee employee)
        {
            var newUser = await _employeeService.CreateEmployee(employee);
            if (newUser == null)
            {
                return Ok("Email Already Exists!");
            }
            return Ok("Employee Added Sucessfully!");
        }

        [HttpPost("addEmployees")]
        public async Task<IActionResult> AddEmployees(NewEmployee[] employees)
        {
            try
            {
                await _employeeService.AddRangeEmployees(employees);
                return Ok("Employees added successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.InnerException.Message}");
            }
        }

        [HttpGet("employee/{id}")]
        public async Task<ActionResult<Employee>> GetEmployeeById(int id)
        {
            var employee = await _employeeService.GetEmployeeById(id);
            if (employee == null)
            {
                throw new ArgumentException(MessageHelper.InvalidUser);
            }
            return employee;
        }

        [HttpDelete("deleteEmployee")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var result = await _employeeService.DeleteEmployee(id);
            if (!result)
            {
                throw new InvalidOperationException(MessageHelper.UserNotRemove);
            }
            return Ok("Employee Deleted");
        }

        [HttpPut("updateEmployee")]
        public async Task<IActionResult> UpdateEmployee(UpdateEmployeeViewModel employee)
        {
            var updatedUser = await _employeeService.UpdateEmployee(employee);
            if (updatedUser != null)
            {
                return Ok("Employee updated successfully");
            }
            else
            {
                return BadRequest("Employee not found");
            }
        }

        [HttpPut("updateRangeEmployees")]
        public async Task<ActionResult<List<EmployeeNotFoundViewModel>>> UpdateEmployees(UpdateEmployeeViewModel[] users)
        {
            var employeeIdsNotFound = await _employeeService.UpdateEmployees(users);

            if (employeeIdsNotFound.Count > 0)
            {
                var errorMessage = "The following employees not found, please enter correct EmployeeId ";
                return BadRequest(new { Message = errorMessage, employeeIdsNotFound });
            }

            return Ok("All Users Updated ");
        }

        [HttpGet("getAllEmployeesName")]
        public async Task<IActionResult> GetEmployeeName()
        {
            var emplyee = await _employeeService.GetEmployeesName();
            return Ok(emplyee);
        }

        [HttpGet("employee/count-by-department")]
        public async Task<ActionResult<Dictionary<string, int>>> GetEmployeeCountByDepartment()
        {
            var countByDepartment = await _employeeService.GetEmployeeCountByDepartment();
            return Ok(countByDepartment);
        }

        //----   SQL - Queries        
        [HttpGet("getEmployeesByDepartment")]
        public async Task<ActionResult<List<DepartmentViewModel>>> GetEmployeesByDepartment()
        {
            List<DepartmentViewModel> departments = await _employeeService.EmployeeByDept();
            return Ok(departments);
        }
                
        [HttpGet("getEmployeesFromHR")]
        public async Task<ActionResult<List<EmployeeViewModel>>> GetEmployeesFromHR()
        {
            List<EmployeeViewModel> employees = await _employeeService.EmployeeFromHR();
            return Ok(employees);
        }
                
        [HttpGet("gerAllHireDates")]
        public async Task<ActionResult<string>> GerAllHireDates()
        {
            string dates = await _employeeService.GerAllHireDates();
            return Ok(dates);
        }
                
        [HttpGet("getEmployeesFirstName")]
        public async Task<ActionResult<string>> GetEmployeesFirstName()
        {
            string names = await _employeeService.GetEmpFirstName();
            return Ok(names);
        }
    }
}
