using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Entities.Model.ViewModel
{
    public class UserSignUpViewModel
    {
        [Required(ErrorMessage = "FirstName is required.")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "LastName is required.")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^[6-9]\d{9}$", ErrorMessage = "Invalid phone number. Phone number must start with 6, 7, 8, or 9 and must be 10 digits.")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(8, ErrorMessage = "Please enter at least 8 characters.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$", ErrorMessage = "Password must be strong.")]
        public string Password { get; set; } = string.Empty;
        [Required,Compare("Password")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
