using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Entities.Model
{
    public class Employee
    {
        public long EmployeeId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string Email { get; set; } = null!;
        public int  Salary { get; set; }
        public string Department { get; set; } = null!;

        public DateTime HireAt { get; set; }

        public bool? Status { get; set; }
    }
}
