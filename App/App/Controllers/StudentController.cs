using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using App.Entities;
using App.Managers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using App.Builders;
using App.Enums;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using App.Models;

namespace App.Controllers
{
    public class StudentController : Controller
    {
        HttpRequestManager _requestFactory;

        public StudentController(IHttpClientFactory httpClientFactory)
        {
            _requestFactory = new HttpRequestManager(httpClientFactory);
        }


        [Authorize(Roles = "Admin, Employee")]
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
                    .AddEntities(EntitiesEnum.Student)
                    .AddRequest(RequestEnum.Get)
                    .AddMethod(MethodEnum.All)
                    .Build();
                var result = await _requestFactory.SendHttpGetRequest(dockerApiUrl);
                if (result == string.Empty)
                {
                    ViewBag.ErrorMessage = "API ile iletişim kurulamadı!";
                    return View();
                }
                var model = JsonConvert.DeserializeObject<List<Student>>(result);
                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Bir hata oluştu: " + ex;
                return View();
            }
        }



        [Authorize(Roles = "Admin, Employee")]
        [HttpGet]
        public async Task<IActionResult> CourseStudents(int CourseID)
        {
            if (_requestFactory == null)
            {
                ViewBag.ErrorMessage = "Talep oluşturulamadı!";
                return View();
            }

            string dockerApiUrl = new ApiUrlBuilder(UrlTypeEnum.api)
                .AddEntities(EntitiesEnum.Course)
                .AddRequest(RequestEnum.Get)
                .AddMethod(MethodEnum.AllStudentWithID)
                .AddParameter(CourseID)
                .Build();
            var result = await _requestFactory.SendHttpGetRequest(dockerApiUrl);
            if (result == string.Empty)
            {
                ViewBag.ErrorMessage = "API ile iletişim kurulamadı!";
                return View();
            }
            List<Student>? model = JsonConvert.DeserializeObject<List<Student>>(result);
            if (model != null)
                return View(model);
            else
                return View();
        }




        // Öncelikle tüm dersler çekilecek. Ve checkbox ile seçilen dersler post işlemi ile gönderilecek:
        // Birer adet Get, Post methodları yazılacak:

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> CourseRegistration()
        {
            string? Number = User.FindFirst(ClaimTypes.Email)?.Value;
            if (Number == null)
            {
                ViewBag.ErrorMessage = "Bir hata oluştu. Oturum açılmamış.";
                return View();
            }

            string dockerApiUrl = new ApiUrlBuilder(UrlTypeEnum.api)
                .AddEntities(EntitiesEnum.Course)
                .AddRequest(RequestEnum.Get)
                .AddMethod(MethodEnum.All)
                .Build();
            var result = await _requestFactory.SendHttpGetRequest(dockerApiUrl);
            if (result == string.Empty)
            {
                ViewBag.ErrorMessage = "API ile iletişim kurulamadı!";
                return View();
            }
            List<Course>? model1 = JsonConvert.DeserializeObject<List<Course>>(result);


            // Öğrencinin hali hazırda aldığı dersler alınacak ve almadığı dersler listelenerek view dosyasına gönderilecek:
            dockerApiUrl = new ApiUrlBuilder(UrlTypeEnum.api)
                .AddEntities(EntitiesEnum.Course)
                .AddRequest(RequestEnum.Get)
                .AddMethod(MethodEnum.WithNumber)
                .AddParameter(Number)
                .Build();
            var result2 = await _requestFactory.SendHttpGetRequest(dockerApiUrl);
            if (result2 == string.Empty)
            {
                ViewBag.ErrorMessage = "API ile iletişim kurulamadı!";
                return View();
            }
            List<Course>? model2 = JsonConvert.DeserializeObject<List<Course>>(result2);

            if (model1 == null || model2 == null)
            {
                ViewBag.ErrorMessage = "Api'den veriler alınamadı.";
                return View();
            }

            for (int i = model1.Count - 1; i >= 0; i--)
            {
                var course = model1[i];
                Course? studentActiveCourses = model2.FirstOrDefault(x => x.ID == course.ID);
                if (studentActiveCourses != null)
                {
                    model1.Remove(course);
                }
            }
            return View(model1);
        }



        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CourseRegistration(List<int> selectedCoursesID)
        {
            string? Number = User.FindFirst(ClaimTypes.Email)?.Value;
            if (Number == null)
            {
                ViewBag.ErrorMessage = "Bir hata oluştu. Oturum açılmamış.";
                return View();
            }

            // CourseRegistration modelinden bir liste oluşturularak o liste gönderilecek.
            List<CourseRegistration> courseList = new List<CourseRegistration>();
            foreach (var courseID in selectedCoursesID)
            {
                CourseRegistration courseRegistration = new CourseRegistration();
                courseRegistration.CourseID = courseID;
                courseList.Add(courseRegistration);
            }
            string dockerApiUrl = new ApiUrlBuilder(UrlTypeEnum.api)
                .AddEntities(EntitiesEnum.Student)
                .AddRequest(RequestEnum.Post)
                .AddMethod(MethodEnum.CourseRegistration)
                .AddParameter(Number)
                .Build();
            string jsonProduct = JsonConvert.SerializeObject(courseList);
            var result = await _requestFactory.SendHttpPostRequest(dockerApiUrl, jsonProduct);
            CourseRegistrationsEnum enumType = JsonConvert.DeserializeObject<CourseRegistrationsEnum>(result);

            if (enumType == CourseRegistrationsEnum.Accept)
            {
                ViewBag.Message = "İşlem başarılı.";
            }
            else if (enumType == CourseRegistrationsEnum.Decline)
            {
                ViewBag.Message = "Talep reddedildi!";
            }
            else if (enumType == CourseRegistrationsEnum.Error)
            {
                ViewBag.Message = "Bir hata oluştu!";
            }
            else if (enumType == CourseRegistrationsEnum.Null)
            {
                ViewBag.Message = "Api hata verdi.";
            }
            else
            {
                ViewBag.Message = "Bilinmeyen bir hata gerçekleşti.";
            }
            return View();
        }




        [Authorize(Roles = "Admin, Employee")]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }


        [Authorize(Roles = "Admin, Employee")]
        [HttpPost]
        public async Task<IActionResult> Create(Student student)
        {
            if (_requestFactory == null)
            {
                ViewBag.ErrorMessage = "Talep oluşturulamadı!";
                return View();
            }
            if (ModelState.IsValid && student != null)
            {
                try
                {
                    string dockerApiUrl = new ApiUrlBuilder(UrlTypeEnum.api)
                        .AddEntities(EntitiesEnum.Student)
                        .AddRequest(RequestEnum.Post)
                        .AddMethod(MethodEnum.Create)
                        .Build();
                    string jsonProduct = JsonConvert.SerializeObject(student);
                    var result = await _requestFactory.SendHttpPostRequest(dockerApiUrl, jsonProduct);
                    CreateReturnEnum createReturnEnum = JsonConvert.DeserializeObject<CreateReturnEnum>(result);

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
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = "Bir hata meydana geldi: " + ex.Message;
                    return View();
                }
            }
            else
            {
                ViewBag.ErrorMessage = "Form doğru formatta değil!";
                return View();
            }
        }



        [Authorize(Roles = "Admin, Employee")]
        [HttpGet]
        public IActionResult Update()
        {
            return View();
        }



        [Authorize(Roles = "Admin, Employee")]
        [HttpPost]
        public async Task<IActionResult> Update(Student student)
        {
            if (ModelState.IsValid)
            {
                if (_requestFactory == null)
                {
                    ViewBag.ErrorMessage = "Talep oluşturulamadı!";
                    return View();
                }
                try
                {
                    string dockerApiUrl = new ApiUrlBuilder(UrlTypeEnum.api)
                        .AddEntities(EntitiesEnum.Student)
                        .AddRequest(RequestEnum.Post)
                        .AddMethod(MethodEnum.Update)
                        .Build();
                    student.IsActive = true;
                    string jsonProduct = JsonConvert.SerializeObject(student);
                    await _requestFactory.SendHttpPostRequest(dockerApiUrl, jsonProduct);

                    ViewBag.Message = "İşlem başarılı.";
                    return View();
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = "Bir hata meydana geldi: " + ex.Message;
                    return View();
                }
            }
            else
            {
                ViewBag.ErrorMessage = "Bilinmeyen bir hata oldu.";
                return View();
            }
        }

    }
}

