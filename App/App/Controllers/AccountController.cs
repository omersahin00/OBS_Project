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
using System.Reflection;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;


namespace App.Controllers
{
	public class AccountController : Controller
	{
        HttpRequestManager _requestFactory;

        public AccountController(IHttpClientFactory httpClientFactory)
        {
            _requestFactory = new HttpRequestManager(httpClientFactory);
        }


        [HttpGet]
        public IActionResult JointLoginPage()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> JointLoginPage(string Number)
        {
            if (_requestFactory == null)
            {
                ViewBag.ErrorMessage = "Talep oluşturulamadı!";
                return View();
            }

            string dockerApiUrl = new ApiUrlBuilder(UrlTypeEnum.api)
                .AddEntities(EntitiesEnum.Account)
                .AddRequest(RequestEnum.Get)
                .AddMethod(MethodEnum.CheckNumber)
                .AddParameter(Number)
                .Build();

            var result = await _requestFactory.SendHttpGetRequest(dockerApiUrl);
            AccountTypeEnum accountType = JsonConvert.DeserializeObject<AccountTypeEnum>(result);

            if (accountType != AccountTypeEnum.Null)
            {
                if (accountType == AccountTypeEnum.Empolyee)
                {
                    ViewBag.Message = "Employe geldi";
                    TempData["NewAccountNumber"] = Number;
                    return RedirectToAction("EmployeeLogin", "Account"); // DAHA YAZILMADI !!!
                }
                else if (accountType == AccountTypeEnum.Student)
                {
                    ViewBag.Message = "Student geldi";
                    TempData["NewAccountNumber"] = Number;
                    return RedirectToAction("StudentLogin", "Account"); // DAHA YAZILMADI !!!
                }
                else
                {
                    ViewBag.ErrorMessage = "Gelen veri tanımsız!";
                    return View();
                }
            }
            else
            {
                ViewBag.ErrorMessage = "Böyle bir kullanıcı mevcut değil.";
                TempData["NewAccountNumber"] = Number;
                return RedirectToAction("Create", "Account");
            }
        }


        [HttpGet]
        public IActionResult EmployeeLogin()
        {
            string? NewAccountNumber = TempData["NewAccountNumber"] as string;
            if (NewAccountNumber != null) ViewBag.NewAccountNumber = NewAccountNumber;
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> EmployeeLogin(Employee employee)
        {
            if (_requestFactory == null)
            {
                ViewBag.ErrorMessage = "Talep oluşturulamadı!";
                return View();
            }
            if (employee.UserNumber == null || employee.Password == null)
            {
                ViewBag.ErrorMessage = "Numara veya şifre girilmemiş!";
                return View();
            }

            string dockerApiUrl = new ApiUrlBuilder(UrlTypeEnum.api)
                .AddEntities(EntitiesEnum.Account)
                .AddRequest(RequestEnum.LoginEmployee)
                .Build();

            string jsonProduct = JsonConvert.SerializeObject(employee);
            var result = await _requestFactory.SendHttpPostRequest(dockerApiUrl, jsonProduct);
            LoginReturnEnum loginReturnType = JsonConvert.DeserializeObject<LoginReturnEnum>(result);

            if (loginReturnType == LoginReturnEnum.Accept)
            {
                ViewBag.Message = "Giriş işlemi başarılı.";
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Email, employee.UserNumber),
                    new Claim(ClaimTypes.Role, EntitiesEnum.Employee.ToString())
                };
                var userIdentity = new ClaimsIdentity(claims, " ");
                var authProporties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMonths(1)
                };
                ClaimsPrincipal principal = new ClaimsPrincipal(userIdentity);
                await HttpContext.SignInAsync(principal, authProporties);
                return RedirectToAction("EmployeeLogin", "Account");
            }
            else if (loginReturnType == LoginReturnEnum.WrongNumber)
            {
                ViewBag.ErrorMessage = "Hatalı numara";
            }
            else if (loginReturnType == LoginReturnEnum.WrongPassword)
            {
                ViewBag.ErrorMessage = "Hatalı şifre";
            }
            else if (loginReturnType == LoginReturnEnum.Null)
            {
                ViewBag.ErrorMessage = "Bilinmeyen bir hata oluştu";
            }
            else
            {
                ViewBag.ErrorMessage = "Hata";
            }


            if (employee.UserNumber != null)
            {
                TempData["NewAccountNumber"] = employee.UserNumber.ToString();
            }
            return View();
        }



        [HttpGet]
        public IActionResult StudentLogin()
        {
            string? NewAccountNumber = TempData["NewAccountNumber"] as string;
            if (NewAccountNumber != null) ViewBag.NewAccountNumber = NewAccountNumber;
            return View();
        }



        [HttpPost]
        public async Task<IActionResult> StudentLogin(Student student)
        {
            if (_requestFactory == null)
            {
                ViewBag.ErrorMessage = "Talep oluşturulamadı!";
                return View();
            }
            if (student.Number == null || student.Password == null)
            {
                ViewBag.ErrorMessage = "Numara veya şifre girilmemiş!";
                return View();
            }

            string dockerApiUrl = new ApiUrlBuilder(UrlTypeEnum.api)
                .AddEntities(EntitiesEnum.Account)
                .AddRequest(RequestEnum.LoginStudent)
                .Build();

            string jsonProduct = JsonConvert.SerializeObject(student);
            var result = await _requestFactory.SendHttpPostRequest(dockerApiUrl, jsonProduct);
            LoginReturnEnum loginReturnType = JsonConvert.DeserializeObject<LoginReturnEnum>(result);

            if (loginReturnType == LoginReturnEnum.Accept)
            {
                ViewBag.Message = "Giriş işlemi başarılı.";
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Email, student.Number),
                    new Claim(ClaimTypes.Role, EntitiesEnum.Student.ToString())
                };
                var userIdentity = new ClaimsIdentity(claims, " ");
                var authProporties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMonths(1)
                };
                ClaimsPrincipal principal = new ClaimsPrincipal(userIdentity);
                await HttpContext.SignInAsync(principal, authProporties);
                return RedirectToAction("StudentLogin", "Account");
            }
            else if (loginReturnType == LoginReturnEnum.WrongNumber)
            {
                ViewBag.ErrorMessage = "Hatalı numara";
            }
            else if (loginReturnType == LoginReturnEnum.WrongPassword)
            {
                ViewBag.ErrorMessage = "Hatalı şifre";
            }
            else if (loginReturnType == LoginReturnEnum.Null)
            {
                ViewBag.ErrorMessage = "Bilinmeyen bir hata oluştu";
            }
            else
            {
                ViewBag.ErrorMessage = "Hata";
            }


            if (student.Number != null)
            {
                TempData["NewAccountNumber"] = student.Number.ToString();
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
                Employee? employeeData = JsonConvert.DeserializeObject<Employee>(result);

                if (employeeData != null)
                {
                    ViewBag.Message = "İşlem başarılı";
                    return View();
                }
                else
                {
                    ViewBag.ErrorMessage = "Böyle bir kullanıcı mevcut!";
                    return View();
                }
            }
            else
            {
                ViewBag.ErrorMessage = "Form doğru formatta değil!";
                return View();
            }
        }


        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("JointLoginPage", "Account");
        }
    }
}

