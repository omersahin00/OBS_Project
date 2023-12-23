using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Entities;
using App.Managers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace App.Controllers
{
    public class StudentController : Controller
    {
        HttpRequestManager _requestFactory;

        public StudentController(IHttpClientFactory httpClientFactory)
        {
            _requestFactory = new HttpRequestManager(httpClientFactory);
        }

        

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
                string dockerApiUrl = "https://localhost:7242/api/Student/";
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



        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        

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
                    string dockerApiUrl = "https://localhost:7242/api/Student";
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



        [HttpGet]
        public IActionResult Update()
        {
            return View();
        }


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
                    string dockerApiUrl = "https://localhost:7242/api/StudentUpdate";
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

