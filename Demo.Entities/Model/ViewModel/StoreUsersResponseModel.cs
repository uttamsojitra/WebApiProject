using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Entities.Model.ViewModel
{
    public class StoreUsersResponseModel
    {
        public List<UserWithNullEmail> UsersWithNullEmail { get; set; }
    }

    public class UserWithNullEmail
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
       
    }
}
