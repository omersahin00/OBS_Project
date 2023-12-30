using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Web_API.Concrete;
using Web_API.Entities;
using Web_API.Models;

namespace Web_API.Controllers
{
    [ApiController]
    public class StudentController : Controller
    {
        protected readonly SqlDbContext _context;

        public StudentController(SqlDbContext context)
        {
            _context = context;
        }


        [HttpGet("api/Student/Get/All")]
        public ActionResult<IEnumerable<Student>> GetAllStudent()
        {
            try
            {
                var students = _context.Students.ToList();
                if (students != null)
                {
                    return Ok(students);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception)
            {
                return UnprocessableEntity();
            }
        }


        [HttpGet("api/Student/Get/WithID/{ID}")]
        public ActionResult<IEnumerable<Student>> GetStudentWithID(int ID)
        {
            try
            {
                var student = _context.Students.FirstOrDefault(x => x.ID == ID);

                List<Student> studentList = new List<Student>();
                if (student != null) studentList.Add(student);

                if (student != null)
                {
                    return Ok(studentList);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception)
            {
                return UnprocessableEntity();
            }
        }



        [HttpGet("api/Student/Get/WithNumber/{Number}")]
        public ActionResult<IEnumerable<Student>> GetStudentWithNumber(string Number)
        {
            try
            {
                var student = _context.Students.FirstOrDefault(x => x.Number == Number);

                List<Student> studentList = new List<Student>();
                if (student != null) studentList.Add(student);

                if (student != null)
                {
                    return Ok(studentList);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception)
            {
                return UnprocessableEntity();
            }
        }



        [HttpGet("api/Student/Get/AllStudentsWithEmployeeNumber/{Number}")]
        public ActionResult<IEnumerable<StudentsWithCourseID>> GetStudentsWithEmployeeNumber(string Number) // Employee numarası alındı.
        {
            // Bu model için belli bir model oluşturulacak. Bu modelde öğrencinin tüm bilgilerinin yanında aldığı dersin de ID'si bulunacak.
            // Birden fazla dersi alan bir öğrenci varsa da o öğrencinin bilgisi gönderilen listede iki kere fakat farklı CourseID'lerine sahip bir şekilde gönderilecek.

            List<EmployeeCourses> employeeCoursesList = _context.EmployeeCourses.Where(x => x.EmployeeNumber == Number).ToList();
            if (employeeCoursesList != null)
            {
                List<StudentsWithCourseID> studentList = new List<StudentsWithCourseID>();

                foreach (var item in employeeCoursesList)
                {
                    List<StudentActiveCourses> studentActiveCourses = _context.StudentActiveCourses.Where(x => x.CourseID == item.CourseID).ToList();
                    foreach (var item2 in studentActiveCourses)
                    {
                        Student? student = _context.Students.FirstOrDefault(x => x.ID == item2.StudentID);
                        if (student != null)
                        {
                            StudentsWithCourseID studentTemp = new StudentsWithCourseID(student);
                            studentTemp.CourseID = item2.CourseID;
                            studentList.Add(studentTemp);
                            // model oluşturuldu ve model listeye eklendi.
                        }
                    }
                }
                // Liste düzenleniyor:
                studentList = studentList.OrderBy(StudentsWithCourseID => StudentsWithCourseID.ID).ToList();
                studentList = studentList.OrderBy(StudentsWithCourseID => StudentsWithCourseID.CourseID).ToList();
                return Ok(studentList);
            }
            else
            {
                return NotFound();
            }
        }



        [HttpPost("api/Student/Post/Create")]
        public ActionResult<Student> CreateStudent(Student student)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Student? studentData = _context.Students.FirstOrDefault(x => x.Number == student.Number);
                    if (studentData != null)
                    {
                        return Ok();
                    }
                    else
                    {
                        student.IsActive = true;
                        _context.Students.Add(student);
                        _context.SaveChanges();
                        return Ok(student);
                    }
                }
                else
                {
                    return UnprocessableEntity();
                }
            }
            catch (Exception)
            {
                return UnprocessableEntity();
            }
        }



        [HttpPost("api/Student/Post/Update")]
        public ActionResult<Student> UpdateStudent(Student student)
        {
            try
            {
                if (ModelState.IsValid && _context.Students != null)
                {
                    var studentData = _context.Students.FirstOrDefault(x => x.Number == student.Number);
                    if (studentData != null)
                    {
                        studentData.StudentNotesID = student.StudentNotesID;
                        studentData.Password = student.Password;
                        studentData.IsActive = student.IsActive;
                        _context.Students.Update(studentData);
                        _context.SaveChanges();
                        return NoContent();
                    }
                    else
                    {
                        return NotFound();
                    }
                }
                else
                {
                    return UnprocessableEntity();
                }
            }
            catch (Exception)
            {
                return UnprocessableEntity();
            }
        }


    }
}

