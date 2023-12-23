using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using App.Managers;
using Newtonsoft.Json;
using App.Entities;
using System.Text;

namespace App.Controllers
{
    public class NotesController : Controller
    {
        HttpRequestManager _requestFactory;

        public NotesController(IHttpClientFactory httpClientFactory)
        {
            _requestFactory = new HttpRequestManager(httpClientFactory);
        }


        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }


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
                string dockerApiUrl = "https://localhost:7242/api/Notes/" + Number;
                var result = await _requestFactory.SendHttpGetRequest(dockerApiUrl);
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

