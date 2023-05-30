using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Entities.Model.ViewModel
{
    public class EmployeeViewModel
    {
        public long EmployeeId { get; set; }
        public string? FullName { get; set; }
       
        public string HireAt { get; set; }
        public int HireYear { get; set; }   

        
    }
}
