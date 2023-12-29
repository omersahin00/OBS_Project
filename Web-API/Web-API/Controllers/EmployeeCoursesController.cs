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


    }
}

