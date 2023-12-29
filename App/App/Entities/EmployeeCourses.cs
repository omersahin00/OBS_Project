using System;
using System.ComponentModel.DataAnnotations;

namespace App.Entities
{
	public class EmployeeCourses
	{
        [Key]
        public int ID { get; set; }

        public int EmployeeID { get; set; }

        public int CourseID { get; set; }
    }
}

