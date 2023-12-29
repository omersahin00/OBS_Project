using System;
using System.ComponentModel.DataAnnotations;

namespace App.Entities
{
	public class EmployeeCourses
	{
        [Key]
        public int ID { get; set; }

        public string? EmployeeNumber { get; set; }

        public int CourseID { get; set; }
    }
}

