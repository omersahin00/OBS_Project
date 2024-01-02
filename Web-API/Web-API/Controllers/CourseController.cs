using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Web_API.Enums;
using Web_API.Concrete;
using Web_API.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Web_API.Controllers
{
    [ApiController]
    public class CourseController : Controller
    {
        protected readonly SqlDbContext _context;

        public CourseController(SqlDbContext context)
        {
            _context = context;
        }


        [HttpGet("api/Course/Get/All")]
        public ActionResult<IEnumerable<Course>> GetAllCourse()
        {
            try
            {
                var courses = _context.Courses.ToList();
                if (courses != null)
                {
                    return Ok(courses);
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



        [HttpGet("api/Course/Get/WithID/{ID}")]
        public ActionResult<IEnumerable<Course>> GetCourseWithID(int ID)
        {
            try
            {
                Course? course = _context.Courses.FirstOrDefault(x => x.ID == ID);
                if (course != null)
                {
                    return Ok(course);
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


        [HttpGet("api/Course/Get/WithNumber/{Number}")]
        public ActionResult<IEnumerable<Course>> GetStudentAllCourses(string Number)
        {
            try
            {
                var student = _context.Students.FirstOrDefault(x => x.Number == Number);
                List<StudentActiveCourses> studentActiveLessonIDList;
                List<Course> studentCourseList = new List<Course>();

                if (student != null && student.ID != 0)
                {
                    studentActiveLessonIDList = _context.StudentActiveCourses.Where(x => x.StudentID == student.ID).ToList();
                    foreach (var item in studentActiveLessonIDList)
                    {
                        var course = _context.Courses.FirstOrDefault(x => x.ID == item.CourseID);
                        if (course != null)
                            studentCourseList.Add(course);
                    }

                    return Ok(studentCourseList);
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



        [HttpGet("api/Course/Get/AllStudentWithID/{CourseID}")]
        public ActionResult<IEnumerable<Student>> GetCourseStudents(string CourseID)
        {
            int _CourseID = int.Parse(CourseID);
            List<StudentActiveCourses> studentCoursesList = _context.StudentActiveCourses.Where(x => x.CourseID == _CourseID).ToList();
            
            if (studentCoursesList != null)
            {
                List<Student> studentList = new List<Student>();
                foreach (var item in studentCoursesList)
                {
                    Student? student = _context.Students.FirstOrDefault(x => x.ID == item.StudentID);
                    if (student != null)
                    {
                        studentList.Add(student);
                    }
                }
                return Ok(studentList);
            }
            else
            {
                return Ok();
            }
        }



        [HttpPost("api/Course/Post/Create")]
        public ActionResult<Course> CreateCourse(Course course)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    course.IsUsed = false;
                    course.IsActive = true;
                    _context.Courses.Add(course);
                    _context.SaveChanges();
                    return Ok(CreateReturnEnum.Accept);
                }
                else
                {
                    return Ok(CreateReturnEnum.Decline);
                }
            }
            catch (Exception)
            {
                return Ok(CreateReturnEnum.Null);
            }
        }



        [HttpPost("api/Course/Post/Update")]
        public ActionResult<Course> UpdateCourse(Course course)
        {
            try
            {
                if (ModelState.IsValid && _context.Courses != null)
                {
                    Course? courseData;

                    if (course.ID != 0)
                        courseData = _context.Courses.FirstOrDefault(x => x.ID == course.ID);

                    else
                        return UnprocessableEntity();

                    if (courseData != null)
                    {
                        courseData.Credit = course.Credit;
                        courseData.IsActive = course.IsActive;
                        _context.Courses.Update(courseData);
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

