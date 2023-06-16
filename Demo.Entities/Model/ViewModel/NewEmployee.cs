using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Entities.Model.ViewModel
{
    public class NewEmployee
    {
        [Required(ErrorMessage = "FirstName is required.")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "LastName is required.")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Salary is required.")]
        public int Salary { get; set; }

        public DateTime HireAt { get; set; }

        [Required(ErrorMessage = "Department is required.")]
        public string Department { get; set; }

        public bool Status { get; set; }    
    }
}
