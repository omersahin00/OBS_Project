using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web_API.Entities
{
	public class StudentActiveCourses
	{
        [Key]
        public int ID { get; set; }

        public int StudentID { get; set; }

        public int CourseID { get; set; }
    }
}

