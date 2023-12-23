using System;
using Microsoft.EntityFrameworkCore;
using Web_API.Entities;

namespace Web_API.Concrete
{
	public class SqlDbContext : DbContext
	{
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=localhost,1433;Initial Catalog=sqlOBS;User ID=SA;Password=reallyStrongPwd123;TrustServerCertificate=true;");
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Notes> Notes { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<StudentActiveLessons> StudentActiveLessons { get; set; }
    }
}

