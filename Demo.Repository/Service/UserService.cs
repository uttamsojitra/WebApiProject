using Demo.Business.Interface;
using Demo.Business.Interface.Interface_Service;
using Demo.Entities.Model;
using Demo.Entities.Model.ViewModel;
using Demo.Repository.Interface;
using iTextSharp.text.pdf;
using OfficeOpenXml;
using Xceed.Document.NET;
using Xceed.Words.NET;

namespace Demo.Repository.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmployeeRepository _employeeRepository;

        public UserService(IUserRepository userRepository, IEmployeeRepository employeeRepository)
        {
            _userRepository = userRepository;
            _employeeRepository = employeeRepository;
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
            return _userRepository.GetTotalUsersCount();
        }

        public async Task<User> CreateUser(UserSignUpViewModel user)
        {
            var newUser = await _userRepository.AddUser(user);
            return newUser;
        }

        public async Task<User> GetEmailAndToken(string email, string token)
        {
            return await _userRepository.GetUserByEmailAndToken(email, token);
        }

        public async Task<User> GetUserStatus(string email, string token)
        {
            return await _userRepository.GetUserStatus(email, token);
        }
        public async Task<User> UpdateUser(User user)
        {
           var users = await _userRepository.UpdateUser(user);
           return users;
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


        //-----  Export to Excel-Pdf-Word  -----
        public async Task<byte[]> ExportUsersDataToExcel()
        {
            var users = await _userRepository.GetUsersData();

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Users");

            // Set the column headers
            worksheet.Cells[1, 1].Value = "First Name";
            worksheet.Cells[1, 2].Value = "Last Name";
            worksheet.Cells[1, 3].Value = "Email";
            worksheet.Cells[1, 4].Value = "Phone Number";

            // Add the user data
            var rowIndex = 2;
            foreach (var user in users)
            {
                worksheet.Cells[rowIndex, 1].Value = user.FirstName;
                worksheet.Cells[rowIndex, 2].Value = user.LastName;
                worksheet.Cells[rowIndex, 3].Value = user.Email;
                worksheet.Cells[rowIndex, 4].Value = user.PhoneNumber;

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

        public async Task<byte[]> ExportUsersDataToPDF()
        {
            var users = await _userRepository.GetUsersData();

            using var memoryStream = new MemoryStream();
            using (var document = new iTextSharp.text.Document())
            {
                PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
                //open the PDF document for writing
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
            return memoryStream.ToArray();

        }

        public async Task<byte[]> ExportUsersDataToWord()
        {
            var users = await _userRepository.GetUsersData();

            using var memoryStream = new MemoryStream();

            using (var document = DocX.Create(memoryStream))
            {
                // Add a table with three columns
                Table table = document.AddTable(users.Count + 1, 3);
                table.Design = TableDesign.TableGrid;

                // Set the column widths (adjust as needed)
                table.SetWidths(new float[] { 90f, 150f, 90f });

                // Set the column headers
                table.Rows[0].Cells[0].Paragraphs.First().Append("User Name").Bold();
                table.Rows[0].Cells[1].Paragraphs.First().Append("Email").Bold();
                table.Rows[0].Cells[2].Paragraphs.First().Append("Phone Number").Bold();

                // Add the user data
                for (int i = 0; i < users.Count; i++)
                {
                    table.Rows[i + 1].Cells[0].Paragraphs.First().Append(users[i].FirstName + " " + users[i].LastName);
                    table.Rows[i + 1].Cells[1].Paragraphs.First().Append(users[i].Email);
                    table.Rows[i + 1].Cells[2].Paragraphs.First().Append(users[i].PhoneNumber);
                }


                document.InsertTable(table);
                //writes the document content to the memoryStream,
                document.Save();
            }
            return memoryStream.ToArray();
        }

        //----  Import excelfile and save data in DataBase   ----
        public async Task<StoreUsersResponseModel> StoreUsersFromExcel(Stream fileStream)
        {
            using var package = new ExcelPackage(fileStream);
            var worksheet = package.Workbook.Worksheets.FirstOrDefault();

            var responseModel = new StoreUsersResponseModel();

            for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
            {
                var email = worksheet.Cells[row, 3].Value?.ToString();

                if (!string.IsNullOrEmpty(email))
                {
                    var user = new User
                    {
                        FirstName = worksheet.Cells[row, 1].Value?.ToString(),
                        LastName = worksheet.Cells[row, 2].Value?.ToString(),
                        Email = email,
                        PhoneNumber = worksheet.Cells[row, 4].Value?.ToString()
                    };

                    // Store the user in the database
                    await _userRepository.AddUserFromFile(user);
                }
                else
                {
                    // Add the user to the response model with null email
                    var userWithNullEmail = new UserWithNullEmail
                    {
                        FirstName = worksheet.Cells[row, 1].Value?.ToString(),
                        LastName = worksheet.Cells[row, 2].Value?.ToString(),

                    };

                    responseModel.UsersWithNullEmail.Add(userWithNullEmail);
                }
            }

            return responseModel;
        }

        public async Task<List<string>> GetAllSkills()
        {
            var skills = await _userRepository.GetSkills();
            return skills;
        }
        //---  Employee-Repo  -----
        public async Task<List<string>> GetEmployeesName()
        {
            return await _employeeRepository.GetEmployeeByName();
        }

        public async Task<Dictionary<string, int>> GetEmployeeCountByDepartment()
        {
            var employees = await _employeeRepository.GetEmployeesAsync();
            var countByDepartment = employees.GroupBy(e => e.Department)
                .ToDictionary(g => g.Key, g => g.Count());

            return countByDepartment;
        }

        public async Task<Employee> GetEmployeeById(int id)
        {
            return await _employeeRepository.GetEmployeeById(id);
        }
    }
}
