using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Entities;
using App.Managers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using App.Builders;
using App.Enums;
using Microsoft.AspNetCore.Authorization;

namespace App.Controllers
{
    public class StudentController : Controller
    {
        HttpRequestManager _requestFactory;

        public StudentController(IHttpClientFactory httpClientFactory)
        {
            _requestFactory = new HttpRequestManager(httpClientFactory);
        }


        [Authorize]
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

