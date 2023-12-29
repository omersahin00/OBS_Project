using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Web_API.Concrete;
using Web_API.Entities;

namespace Web_API.Controllers
{
    [ApiController]
    //[Route("api/[controller]")]
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



        [HttpGet("api/Student/Get/WithID/{Number}")]
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

