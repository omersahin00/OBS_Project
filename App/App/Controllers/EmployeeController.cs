using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using App.Enums;
using App.Models;
using App.Builders;
using App.Managers;
using App.Entities;
using Newtonsoft.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace App.Controllers
{
    public class EmployeeController : Controller
    {
        HttpRequestManager _requestFactory;

        public EmployeeController(IHttpClientFactory httpClientFactory)
        {
            _requestFactory = new HttpRequestManager(httpClientFactory);
        }


        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (_requestFactory == null)
            {
                ViewBag.ErrorMessage = "Talep oluşturulamadı!";
                return View();
            }
            try
            {
                string dockerApiUrl = new ApiUrlBuilder(UrlTypeEnum.api)
                    .AddEntities(EntitiesEnum.Employee)
                    .AddRequest(RequestEnum.Get)
                    .AddMethod(MethodEnum.All)
                    .Build();
                var result = await _requestFactory.SendHttpGetRequest(dockerApiUrl);
                if (result == string.Empty)
                {
                    ViewBag.ErrorMessage = "API ile iletişim kurulamadı!";
                    return View();
                }
                List<Employee>? model = JsonConvert.DeserializeObject<List<Employee>>(result);
                if (model != null)
                {
                    return View(model);
                }
                else
                {
                    ViewBag.ErrorMessage = "Api ile ilgili bir sorun oluştu.";
                    return View();
                }
            }
            catch (Exception)
            {
                ViewBag.ErrorMessage = "Bilinmeyen bir hata oluştu.";
                return View();
            }
        }



        [Authorize(Roles = "Admin, Employee")]
        [HttpGet]
        public async Task<IActionResult> EmployeeStudents(string? Number)
        {
            if (_requestFactory == null)
            {
                ViewBag.ErrorMessage = "Talep oluşturulamadı!";
                return View();
            }

            if (Number == null)
            {
                Number = User.FindFirst(ClaimTypes.Email)?.Value;
                if (Number == null)
                {
                    ViewBag.ErrorMessage = "Oturum Açılmamış!";
                    return View();
                }
            }


            // Öğrenciler çekiliyor:
            string dockerApiUrl1 = new ApiUrlBuilder(UrlTypeEnum.api)
                .AddEntities(EntitiesEnum.Student)
                .AddRequest(RequestEnum.Get)
                .AddMethod(MethodEnum.AllStudentsWithEmployeeNumber)
                .AddParameter(Number)
                .Build();
            var result1 = await _requestFactory.SendHttpGetRequest(dockerApiUrl1);
            if (result1 == string.Empty)
            {
                ViewBag.ErrorMessage = "API ile iletişim kurulamadı!";
                return View();
            }
            List<StudentsWithCourseID>? model1 = JsonConvert.DeserializeObject<List<StudentsWithCourseID>>(result1);


            // Kurslar çekiliyor:
            string dockerApiUrl2 = new ApiUrlBuilder(UrlTypeEnum.api)
                .AddEntities(EntitiesEnum.Course)
                .AddRequest(RequestEnum.Get)
                .AddMethod(MethodEnum.All)
                .Build();
            var result2 = await _requestFactory.SendHttpGetRequest(dockerApiUrl2);
            if (result2 == string.Empty)
            {
                ViewBag.ErrorMessage = "API ile iletişim kurulamadı!";
                return View();
            }
            List<Course>? model2 = JsonConvert.DeserializeObject<List<Course>>(result2);

            if (model1 == null || model2 == null)
            {
                ViewBag.ErrorMessage = "Api ile ilgili bir sorun oluştu.";
                return View();
            }

            List<StudentAndCourse> studentAndCoursesList = new List<StudentAndCourse>();

            foreach (var item in model1)
            {
                Course? course = model2.FirstOrDefault(x => x.ID == item.CourseID);
                if (course != null)
                {
                    StudentAndCourse studentAndCourse = new StudentAndCourse(item, course);
                    studentAndCoursesList.Add(studentAndCourse);
                }
            }
            return View(studentAndCoursesList);
        }



        [Authorize(Roles = "Admin, Employee")]
        [HttpGet]
        public async Task<IActionResult> Courses()
        {
            if (_requestFactory == null)
            {
                ViewBag.ErrorMessage = "Talep oluşturulamadı!";
                return View();
            }

            string? Number = User.FindFirst(ClaimTypes.Email)?.Value;

            if (Number == null)
            {
                ViewBag.ErrorMessage = "Kullanıcı bulunamadı!";
                return View();
            }

            string dockerApiUrl = new ApiUrlBuilder(Enums.UrlTypeEnum.api)
                .AddEntities(EntitiesEnum.EmployeeCourses)
                .AddRequest(RequestEnum.Get)
                .AddMethod(MethodEnum.WithNumber)
                .AddParameter(Number)
                .Build();
            var result = await _requestFactory.SendHttpGetRequest(dockerApiUrl);
            if (result == string.Empty)
            {
                ViewBag.ErrorMessage = "API ile iletişim kurulamadı!";
                return View();
            }
            List<Course>? courseList = JsonConvert.DeserializeObject<List<Course>>(result);
            if (courseList != null)
            {
                return View(courseList);
            }
            return View();
        }








        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Create()
        {
            string? NewAccountNumber = TempData["NewAccountNumber"] as string;
            if (NewAccountNumber != null) ViewBag.NewAccountNumber = NewAccountNumber;
            return View();
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(Employee employee)
        {
            if (_requestFactory == null)
            {
                ViewBag.ErrorMessage = "Talep oluşturulamadı!";
                return View();
            }

            if (ModelState.IsValid && employee != null)
            {
                string dockerApiUrl = new ApiUrlBuilder(UrlTypeEnum.api)
                    .AddEntities(EntitiesEnum.Employee)
                    .AddRequest(RequestEnum.Post)
                    .AddMethod(MethodEnum.Create)
                    .Build();

                string jsonProduct = JsonConvert.SerializeObject(employee);
                var result = await _requestFactory.SendHttpPostRequest(dockerApiUrl, jsonProduct);
                if (result == string.Empty)
                {
                    ViewBag.ErrorMessage = "API ile iletişim kurulamadı!";
                    return View();
                }
                CreateReturnEnum? createReturnEnum = JsonConvert.DeserializeObject<CreateReturnEnum>(result);

                if (createReturnEnum == null)
                {
                    ViewBag.ErrorMessage = "Api cevap vermedi.";
                    return View();
                }

                if (createReturnEnum == CreateReturnEnum.Accept)
                {
                    ViewBag.Message = "İşlem başarılı.";
                }
                else if (createReturnEnum == CreateReturnEnum.Conflict)
                {
                    ViewBag.ErrorMessage = "Böyle bir kullanıcı zaten mevcut.";
                }
                else if (createReturnEnum == CreateReturnEnum.Null)
                {
                    ViewBag.ErrorMessage = "İşlem gerçekleştirilemedi!";
                }
                else
                {
                    ViewBag.ErrorMessage = "Bilinmeyen bir hata oluştu.";
                }
                return View();
            }
            else
            {
                ViewBag.ErrorMessage = "Form doğru formatta değil!";
                return View();
            }
        }
    }
}

