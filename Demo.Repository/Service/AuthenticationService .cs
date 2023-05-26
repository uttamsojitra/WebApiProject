using Demo.Business.Interface.Interface_Service;
using Demo.Entities.Model;
using Demo.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Business.Service
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserRepository _userRepository;

        public AuthenticationService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User> GetAuthUser(string FirstName, string Password)
        {
            var user = await _userRepository.GetAuthUser(FirstName, Password);  
            return user;             
        }

    }
}
