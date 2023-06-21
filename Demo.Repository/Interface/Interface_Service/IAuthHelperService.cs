using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Business.Interface.Interface_Service
{
    public interface IAuthHelperService
    {
        int UserId { get; }
       
        public List<Tuple<string, bool>> Permissions { get; }
    }
}
