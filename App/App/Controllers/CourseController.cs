using System;
using App.Models;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using App.Enums;
using App.Entities;
using App.Managers;
using App.Builders;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace App.Controllers
{
    public class CourseController : Controller
    {
        HttpRequestManager _requestFactory;

        public CourseController(IHttpClientFactory httpClientFactory)
        {
            _requestFactory = new HttpRequestManager(httpClientFactory);
        }


        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            // Tüm kurslar alınıp View'e gönderilecek:
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
            List<Course>? allCourseList = JsonConvert.DeserializeObject<List<Course>>(result);
            return View(allCourseList);
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(Course course)
        {
            if (course == null)
            {
                ViewBag.ErrorMessage = "Bir sorun oluştu.";
                return View();
            }
            if (ModelState.IsValid)
            {
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
                List<Course>? allCourseList = JsonConvert.DeserializeObject<List<Course>>(result);
                if (allCourseList == null)
                {
                    ViewBag.ErrorMessage = "API ile iligili bir sorun oluştu.";
                    return View();
                }
                Course? courseData = allCourseList.FirstOrDefault(x => (x.Name?.ToUpper().ToLower()) == (course.Name?.ToUpper().ToLower()));
                if (courseData != null)
                {
                    ViewBag.ErrorMessage = "Böyle bir kurs zaten mevcut!";
                    return View();
                }
                dockerApiUrl = new ApiUrlBuilder(UrlTypeEnum.api)
                    .AddEntities(EntitiesEnum.Course)
                    .AddRequest(RequestEnum.Post)
                    .AddMethod(MethodEnum.Create)
                    .Build();
                string jsonProduct = JsonConvert.SerializeObject(course);
                var result2 = await _requestFactory.SendHttpPostRequest(dockerApiUrl, jsonProduct);
                if (result2 == string.Empty)
                {
                    ViewBag.ErrorMessage = "API ile iletişim kurulamadı!";
                    return View();
                }
                CreateReturnEnum? createReturnEnum = JsonConvert.DeserializeObject<CreateReturnEnum>(result2);
                if (createReturnEnum == null)
                {
                    ViewBag.ErrorMessage = "API ile iletişime geçilemedi!";
                    return View();
                }

                if (createReturnEnum == CreateReturnEnum.Accept)
                {
                    ViewBag.Message = "İşlem başarılı.";
                }
                else if (createReturnEnum == CreateReturnEnum.Decline)
                {
                    ViewBag.ErrorMessage = "İşlem reddedildi.";
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
                ViewBag.ErrorMessage = "Form doğru formatta değil.";
                return View();
            }
        }




    }
}

