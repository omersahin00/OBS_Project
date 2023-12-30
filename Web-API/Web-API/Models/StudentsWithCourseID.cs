using System;
using System.ComponentModel.DataAnnotations;
using Web_API.Entities;

namespace Web_API.Models
{
	public class StudentsWithCourseID : Student
	{
        // Diğer öğeler Student'dan miras alınıyor.

        public StudentsWithCourseID(Student student)
        {
            ID = student.ID;
            Number = student.Number;
            FirstName = student.FirstName;
            LastName = student.LastName;
            StudentNotesID = student.StudentNotesID;
            Password = student.Password;
            IsActive = student.IsActive;
        }

        public int CourseID { get; set; }
    }
}

