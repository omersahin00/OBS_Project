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
    public class EmployeeCoursesController : Controller
    {
        protected readonly SqlDbContext _context;

        public EmployeeCoursesController(SqlDbContext context)
        {
            _context = context;
        }

        
        [HttpGet("api/EmployeeCourses/Get/WithNumber/{Number}")]
        public ActionResult<IEnumerable<Course>> GetEmployeeCourses(string Number)
        {
            // Öğretmenin verdiği derslerin listesi:
            List<EmployeeCourses> employeeCourses = _context.EmployeeCourses.Where(x => x.EmployeeNumber == Number).ToList();
            List<Course> courseList = new List<Course>();

            if (employeeCourses != null)
            {
                foreach (var item in employeeCourses)
                {
                    Course? course = _context.Courses.FirstOrDefault(x => x.ID == item.CourseID);
                    if (course != null)
                    {
                        courseList.Add(course);
                    }
                }
                return Ok(courseList);
            }
            else
            {
                return Ok();
            }
        }



        [HttpPost("api/EmployeeCourses/Post/CreateWithNumber/{Number}")]
        public ActionResult<CreateReturnEnum> CreateEmployeeCourse(string Number, List<int> CourseIDList)
        {
            try
            {
                Employee? employeeData = _context.Employees.FirstOrDefault(x => x.UserNumber == Number);
                if (CourseIDList == null || employeeData == null)
                {
                    return CreateReturnEnum.Decline;
                }

                foreach (int courseID in CourseIDList)
                {
                    Course? course = _context.Courses.FirstOrDefault(x => x.ID == courseID);
                    if (course == null) return CreateReturnEnum.Null;
                    course.IsUsed = true;
                    _context.Courses.Update(course);

                    EmployeeCourses employeeCourse = new EmployeeCourses();
                    employeeCourse.EmployeeNumber = Number;
                    employeeCourse.CourseID = courseID;
                    _context.EmployeeCourses.Add(employeeCourse);
                }
                _context.SaveChanges();
                
                return CreateReturnEnum.Accept;
            }
            catch (Exception)
            {
                return CreateReturnEnum.Null;
            }
        }


    }
}

