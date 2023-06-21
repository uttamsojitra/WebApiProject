using Demo.Entities.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserDemo.Data.migrations;

namespace Demo.Entities.Data
{
    public class UserDbcontext : DbContext
    {
        public UserDbcontext(DbContextOptions<UserDbcontext> options)
        : base(options)
        {
        }

        public virtual DbSet<Demomodel> Demomodels { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
    }
}
