using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Entities.Model
{
    public class User
    {
        public long UserId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; }
        public string Password { get; set; } = null!;

        public bool Status { get; set; }    

        public string? Token { get; set; }   

    }
}
