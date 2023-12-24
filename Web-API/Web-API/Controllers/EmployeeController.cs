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
    public class EmployeeController : Controller
    {
        protected readonly SqlDbContext _context;

        public EmployeeController(SqlDbContext context)
        {
            _context = context;
        }



        [HttpGet("api/Employee/Get/All")]
        public ActionResult<IEnumerable<Employee>> GetAllEmployee()
        {
            try
            {
                var employees = _context.Employees.ToList();
                if (employees != null)
                {
                    return Ok(employees);
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


        [HttpGet("api/Employee/Get/WithID/{ID}")]
        public ActionResult<IEnumerable<Employee>> GetEmployeeWithID(int ID)
        {
            try
            {
                var employee = _context.Employees.FirstOrDefault(x => x.ID == ID);

                List<Employee> employeeList = new List<Employee>();
                if (employee != null) employeeList.Add(employee);

                if (employee != null)
                {
                    return Ok(employeeList);
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


        [HttpGet("api/Employee/Get/WithNumber/{Number}")]
        public ActionResult<IEnumerable<Employee>> GetEmployeeWithNumber(string Number)
        {
            try
            {
                var employee = _context.Employees.FirstOrDefault(x => x.UserNumber == Number);

                List<Employee> employeeList = new List<Employee>();
                if (employee != null) employeeList.Add(employee);

                if (employee != null)
                {
                    return Ok(employeeList);
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



        [HttpPost("api/Employee/Post/Create")]
        public ActionResult<Employee> CreateEmployee(Employee employee)
        {
            if (ModelState.IsValid)
            {
                Employee? employeeData = _context.Employees.FirstOrDefault(x => x.UserNumber == employee.UserNumber);
                if (employeeData != null)
                {
                    return Ok();
                }
                else
                {
                    employee.IsActive = true;
                    _context.Employees.Add(employee);
                    _context.SaveChanges();
                    return Ok(employee);
                }
            }
            else
            {
                return UnprocessableEntity();
            }
        }



        [HttpPost("api/Employee/Post/Update")]
        public ActionResult<Employee> UpdateEmployee(Employee employee)
        {
            try
            {
                if (ModelState.IsValid && _context.Employees != null)
                {
                    var employeeData = _context.Employees.FirstOrDefault(x => x.UserNumber == employee.UserNumber);
                    if (employeeData != null)
                    {
                        employeeData.Password = employee.Password;
                        employeeData.IsActive = employee.IsActive;
                        _context.Employees.Update(employeeData);
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

