using System;
using Microsoft.EntityFrameworkCore;
using Web_API.Entities;

namespace Web_API.Concrete
{
	public class SqlDbContext : DbContext
	{
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=obs-sql-server,1433;Initial Catalog=sqlOBS;User ID=SA;Password=reallyStrongPwd123;TrustServerCertificate=true;");
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Notes> Notes { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<StudentActiveCourses> StudentActiveCourses { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<EmployeeCourses> EmployeeCourses { get; set; }
    }
}

