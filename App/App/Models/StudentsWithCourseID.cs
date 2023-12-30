using System;
using System.ComponentModel.DataAnnotations;
using App.Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace App.Models
{
	public class StudentsWithCourseID
	{
        [Key]
        public int ID { get; set; }

        public string? Number { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public int StudentNotesID { get; set; }

        public string? Password { get; set; }

        public bool IsActive { get; set; }

        public int CourseID { get; set; }
    }
}

