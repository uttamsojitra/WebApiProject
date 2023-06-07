using Demo.Business.Exception;
using Demo.Business.Interface.Interface_Service;
using Demo.Entities.Model;
using Demo.Entities.Model.ViewModel;
using Demo.Repository.Interface;
using Demo.Repository.Repository;
using iTextSharp.text;
using iTextSharp.text.pdf;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Repository.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        //-------    User CRUD operation    -------
        public async Task<User> GetUserById(int id)
        {
            return await _userRepository.GetUser(id);
        }

        public async Task<IEnumerable<User>> GetAllUsers(int pageNumber, int pageSize)
        {
            var users = await _userRepository.GetUserList();
            var usersPerPage = users.Skip((pageNumber - 1) * pageSize).Take(pageSize);
            return usersPerPage;
        }

      
        public int GetTotalPages(int pageSize)
        {
            var totalUsers = _userRepository.GetTotalUsersCount();
            return (int)Math.Ceiling((decimal)totalUsers / pageSize);
        }

        public int GetTotalUsersCount()
        {
            return  _userRepository.GetTotalUsersCount();
        }

        public async Task<User> CreateUser(UserSignUpViewModel user)
        {
            var newUser = await _userRepository.AddUser(user);
            return newUser;
        }

        public async Task<User> GetEmailAndToken(string email, string token)
        {
            return await  _userRepository.GetUserByEmailAndToken(email, token);
        }

        public async Task<User> GetUserStatus(string email, string token)
        {
            return await _userRepository.GetUserStatus(email, token);
        }
        public async Task UpdateUser(User user)
        {
            await _userRepository.UpdateUser(user);
        }

        public async Task<bool> DeleteUser(int id)
        {
            var result = await _userRepository.RemoveUser(id);
            if (!result)
            {
               return false;
            }
            return true;    
        }

        //-------    Sql queries on Employee table    -------
        public async Task<List<DepartmentViewModel>> EmployeeByDept()
        {
           return await _userRepository.EmpByDepartment();
        } 
        public async Task<List<EmployeeViewModel>> EmployeeFromHR()
        {
           return await _userRepository.EmployeeFromHR();
        }
        public async Task<string> GerAllHireDates()
        {
            return await _userRepository.GetHiringDates();
        }
        public async Task<string> GetEmpFirstName()
        {
            return await _userRepository.GetAllFirstName();
        }

        public async Task<byte[]> ExportUsersDataToExcel()
        {
            try
            {
                var users = await _userRepository.GetUsersData();

                using var package = new ExcelPackage();
                var worksheet = package.Workbook.Worksheets.Add("Users");

                // Set the column headers
                worksheet.Cells[1, 1].Value = "User Name";
                worksheet.Cells[1, 2].Value = "Email";
                worksheet.Cells[1, 3].Value = "Phone Number";

                // Add the user data
                var rowIndex = 2;
                foreach (var user in users)
                {
                    worksheet.Cells[rowIndex, 1].Value = user.FirstName +" "+ user.LastName;
                    worksheet.Cells[rowIndex, 2].Value = user.Email;
                    worksheet.Cells[rowIndex, 3].Value = user.PhoneNumber;

                    rowIndex++;
                }

                // Auto-fit columns
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                // Set left alignment for the phone number column
                var phoneColumn = worksheet.Column(3);
                phoneColumn.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;

                //ExcelPackage is converted to a byte array 
                return package.GetAsByteArray();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public async Task<byte[]> ExportUsersDataToPDF()
        {
        try
        {
            var users = await _userRepository.GetUsersData();

            using var memoryStream = new MemoryStream();
            using (var document = new Document())
            {
                PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
                document.Open();

                // Create the table with three columns
                PdfPTable table = new(3);

                // Set the column widths (adjust as needed)
                float[] columnWidths = new float[] { 1f, 1.5f, 1f };
                table.SetWidths(columnWidths);

                // Set the column headers
                table.AddCell("User Name");
                table.AddCell("Email");
                table.AddCell("Phone Number");

                    
                // Add the user data
                foreach (var user in users)
                {
                    table.AddCell(user.FirstName + " " + user.LastName);
                    table.AddCell(user.Email);
                    table.AddCell(user.PhoneNumber);
                }

                // Add the table to the document
                document.Add(table);

                document.Close();
                writer.Close();
            }

            // Convert the memory stream to a byte array
            return memoryStream.ToArray();
        }
        catch (Exception ex)
         {
             throw new Exception(ex.Message);
         }
        }

  }
}
