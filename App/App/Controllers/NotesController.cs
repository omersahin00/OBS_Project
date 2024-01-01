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
            if (Number != null) return await GetIndexViewPage(Number);
            else return View();
        }



        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Index(string Number)
        {
            if (Number != null) return await GetIndexViewPage(Number);
            else return View();
        }



        // Bu bir fonksiyon:
        public async Task<IActionResult> GetIndexViewPage(string Number)
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




        [Authorize(Roles = "Admin, Employee")]
        [HttpGet]
        public async Task<IActionResult> NoteUpdate(string CourseID)
        {
            if (CourseID == null)
            {
                ViewBag.ErrorMessage = "Bir sorun oluştu!";
                return View();
            }

            int _CourseID = int.Parse(CourseID);

            // Öğrenci Listesi Çekiliyor:
            string dockerApiUrl = new ApiUrlBuilder(UrlTypeEnum.api)
                   .AddEntities(EntitiesEnum.Course)
                   .AddRequest(RequestEnum.Get)
                   .AddMethod(MethodEnum.AllStudentWithID)
                   .AddParameter(CourseID)
                   .Build();
            var result1 = await _requestFactory.SendHttpGetRequest(dockerApiUrl);
            if (result1 == string.Empty)
            {
                ViewBag.ErrorMessage = "API ile iletişim kurulamadı!";
                return View();
            }
            List<Student>? studentList = JsonConvert.DeserializeObject<List<Student>>(result1);

            if (studentList == null)
            {
                ViewBag.ErrorMessage = "Öğrenci listesi alınamadı.";
                return View();
            }

            List<NoteUpdateModel> noteModelList = new List<NoteUpdateModel>();

            // Notlar Listesi Çekiliyor:
            foreach (var student in studentList)
            {
                if (student.Number == null) continue;
                dockerApiUrl = new ApiUrlBuilder(UrlTypeEnum.api)
                    .AddEntities(EntitiesEnum.Notes)
                    .AddRequest(RequestEnum.Get)
                    .AddMethod(MethodEnum.AllWithNumber)
                    .AddParameter(student.Number)
                    .Build();
                var result2 = await _requestFactory.SendHttpGetRequest(dockerApiUrl);
                if (result2 == string.Empty)
                {
                    ViewBag.ErrorMessage = "API ile iletişim kurulamadı!";
                    return View();
                }
                List<Notes>? noteList = JsonConvert.DeserializeObject<List<Notes>>(result2);

                Notes? studentNote = noteList?.FirstOrDefault(x => x.CourseID == _CourseID);
                if (studentNote != null)
                {
                    NoteUpdateModel noteUpdateModel = new NoteUpdateModel();
                    noteUpdateModel.NoteID = studentNote.ID;
                    noteUpdateModel.StudentNumber = student.Number;
                    noteUpdateModel.FirstName = student.FirstName;
                    noteUpdateModel.LastName = student.LastName;
                    noteUpdateModel.Score = studentNote.Score;
                    noteUpdateModel.LetterScore = studentNote.LetterScore;
                    noteModelList.Add(noteUpdateModel);
                }
            }

            ViewBag.Message = JsonConvert.SerializeObject(noteModelList);
            return View(noteModelList);
        }




        [Authorize(Roles = "Admin, Employee")]
        [HttpPost]
        public async Task<IActionResult> NoteUpdate(List<NoteUpdateModel> noteList)
        {
            if (noteList == null)
            {
                ViewBag.ErrorMessage = "Bir sorun oluştu.";
                return View();
            }

            string metin = "";

            List<Notes> newNoteList = new List<Notes>();
            foreach (var studentNote in noteList)
            {
                Notes note = new Notes();
                note.ID = studentNote.NoteID;
                note.Score = studentNote.Score;
                note.LetterScore = LetterScoreManager.LetterScoreCalculator(studentNote.Score);

                metin += note.Score + " " + note.LetterScore + " - ";

                note.IsActive = true;
                newNoteList.Add(note);
            }

            string dockerApiUrl = new ApiUrlBuilder(UrlTypeEnum.api)
                .AddEntities(EntitiesEnum.Notes)
                .AddRequest(RequestEnum.Post)
                .AddMethod(MethodEnum.UpdateList)
                .Build();
            string jsonProduct = JsonConvert.SerializeObject(newNoteList);
            var result = await _requestFactory.SendHttpPostRequest(dockerApiUrl, jsonProduct);
            if (result == string.Empty)
            {
                ViewBag.ErrorMessage = "API ile iletişim kurulamadı!";
                return View();
            }
            UpdateReturnEnum? updateReturnEnum = JsonConvert.DeserializeObject<UpdateReturnEnum>(result);
            if (updateReturnEnum != UpdateReturnEnum.Accept)
            {
                ViewBag.ErrorMessage = "Bir hata oluştu." + " " + updateReturnEnum;
                ViewBag.Message = jsonProduct;
                TempData["ErrorMessage"] = metin;
                return View();
            }

            ViewBag.Message = "İşlem tamamlandı.";
            ViewBag.ErrorMessage = metin;
            return View();
        }


    }
}

