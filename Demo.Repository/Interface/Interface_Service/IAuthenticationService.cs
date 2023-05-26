using Demo.Entities.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Business.Interface.Interface_Service
{
    public interface IAuthenticationService
    {
        public Task<User> GetAuthUser(string FirstName, string Password);
    }

}
