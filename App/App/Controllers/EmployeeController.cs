using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using App.Builders;
using App.Managers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using App.Enums;
using App.Entities;
using Newtonsoft.Json;

namespace App.Controllers
{
    public class EmployeeController : Controller
    {
        HttpRequestManager _requestFactory;

        public EmployeeController(IHttpClientFactory httpClientFactory)
        {
            _requestFactory = new HttpRequestManager(httpClientFactory);
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


    }
}

