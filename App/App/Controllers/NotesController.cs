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
using System.Security.Claims;
using App.Models;

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
        public async Task<IActionResult> Index()
        {
            string? Number = User.FindFirst(ClaimTypes.Email)?.Value;
            if (Number != null) return await GetViewPage(Number);
            else return View();
        }



        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Index(string Number)
        {
            if (Number != null) return await GetViewPage(Number);
            else return View();
        }



        // Bu bir fonksiyon:
        public async Task<IActionResult> GetViewPage(string Number)
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
                    .AddMethod(MethodEnum.AllWithNumber)
                    .AddParameter(Number)
                    .Build();
                var result1 = await _requestFactory.SendHttpGetRequest(dockerApiUrl);
                if (result1 == string.Empty)
                {
                    ViewBag.ErrorMessage = "API ile iletişim kurulamadı!";
                    return View();
                }
                List<Notes>? noteList = JsonConvert.DeserializeObject<List<Notes>>(result1);


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
                List<Course>? courseList = JsonConvert.DeserializeObject<List<Course>>(result2);


                List<StudentNotesModel> modelList = new List<StudentNotesModel>();

                if (noteList != null && courseList != null)
                {
                    foreach (var note in noteList)
                    {
                        StudentNotesModel model = new StudentNotesModel { };
                        Course? course = courseList.FirstOrDefault(x => x.ID == note.CourseID);
                        if (course != null)
                        {
                            model.Name = course.Name;
                            model.Credit = course.Credit;
                        }
                        model.Score = note.Score;
                        model.LetterScore = note.LetterScore;
                        model.IsActive = note.IsActive;
                        modelList.Add(model);
                    }
                }

                return View(modelList);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Bir hata oluştu: " + ex;
                return View();
            }
        }
    }
}

