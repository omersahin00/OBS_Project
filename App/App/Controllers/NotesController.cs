using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using App.Managers;
using Newtonsoft.Json;
using App.Entities;
using App.Builders;
using App.Enums;
using Microsoft.AspNetCore.Authorization;

namespace App.Controllers
{
    public class NotesController : Controller
    {
        HttpRequestManager _requestFactory;

        public NotesController(IHttpClientFactory httpClientFactory)
        {
            _requestFactory = new HttpRequestManager(httpClientFactory);
        }


        [Authorize]
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }


        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Index(string Number)
        {
            if (_requestFactory == null)
            {
                ViewBag.ErrorMessage = "Talep oluşturulamadı!";
                return View();
            }
            try
            {
                string dockerApiUrl = new ApiUrlBuilder(UrlTypeEnum.api)
                    .AddEntities(EntitiesEnum.Notes)
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

                var model = JsonConvert.DeserializeObject<List<Notes>>(result);
                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Bir hata oluştu: " + ex;
                return View();
            }
        }


    }
}

