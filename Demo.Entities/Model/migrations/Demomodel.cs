using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserDemo.Data.migrations
{
    public class Demomodel
    {
        [Key]
        public string? FullName { get; set; }
        public string? Sales { get; set; }
        public string? IT { get; set; }
        public string? HR { get; set; }

    }
}
