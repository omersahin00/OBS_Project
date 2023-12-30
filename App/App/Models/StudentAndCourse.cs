using System;
using System.ComponentModel.DataAnnotations;
using App.Entities;

namespace App.Models
{
	public class StudentAndCourse
	{
        public int StudentID { get; set; }

        public string? Number { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public int StudentNotesID { get; set; }

        public string? Password { get; set; }

        public bool IsActive { get; set; }

        public int CourseID { get; set; }

        public string? CourseName { get; set; }

        public int CourseCredit { get; set; }


        public StudentAndCourse(StudentsWithCourseID student, Course course)
		{
            StudentID = student.ID;
            Number = student.Number;
            FirstName = student.FirstName;
            LastName = student.LastName;
            StudentNotesID = student.StudentNotesID;
            Password = student.Password;
            IsActive = student.IsActive;

            CourseID = course.ID;
            CourseName = course.Name;
            CourseCredit = course.Credit;
        }
	}
}

