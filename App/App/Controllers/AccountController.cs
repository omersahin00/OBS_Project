using System;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;
using App.Enums;
using App.Managers;
using App.Entities;
using App.Builders;
using Newtonsoft.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
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
            if (result == string.Empty)
            {
                ViewBag.ErrorMessage = "API ile iletişim kurulamadı!";
                return View();
            }

            AccountTypeEnum accountType = JsonConvert.DeserializeObject<AccountTypeEnum>(result);

            if (accountType != AccountTypeEnum.TryCatchError)
            {
                if (accountType == AccountTypeEnum.Empolyee)
                {
                    TempData["NewAccountNumber"] = Number;
                    return RedirectToAction("EmployeeLogin", "Account");
                }
                else if (accountType == AccountTypeEnum.Student)
                {
                    TempData["NewAccountNumber"] = Number;
                    return RedirectToAction("StudentLogin", "Account");
                }
                else if (accountType == AccountTypeEnum.Null)
                {
                    // Buraya yetki olmadan da girilebiliyor.
                    // Güvenlik açığı var. Buraya sadece Admin girebilmeli:
                    TempData["ErrorMessage"] = "Böyle bir kullanıcı mevcut değil.";
                    TempData["NewAccountNumber"] = Number;
                    //return RedirectToAction("Create", "Employee");
                    return View();
                }
                else
                {
                    ViewBag.ErrorMessage = "Gelen veri tanımsız!";
                    return View();
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Bir hata meydana geldi!";
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
            if (result == string.Empty)
            {
                ViewBag.ErrorMessage = "API ile iletişim kurulamadı!";
                return View();
            }

            LoginReturnEnum loginReturnType = JsonConvert.DeserializeObject<LoginReturnEnum>(result);

            if (loginReturnType == LoginReturnEnum.Accept)
            {
                ViewBag.Message = "Giriş işlemi başarılı.";

                dockerApiUrl = new ApiUrlBuilder(UrlTypeEnum.api)
                    .AddEntities(EntitiesEnum.Employee)
                    .AddRequest(RequestEnum.Get)
                    .AddMethod(MethodEnum.WithNumber)
                    .AddParameter(employee.UserNumber)
                    .Build();
                var result2 = await _requestFactory.SendHttpGetRequest(dockerApiUrl);
                if (result2 == string.Empty)
                {
                    ViewBag.ErrorMessage = "API ile iletişim kurulamadı!1";
                    return View();
                }
                List<Employee>? employee1 = JsonConvert.DeserializeObject<List<Employee>>(result2);


                RoleEnum roleEnum;

                if (employee.UserNumber == "1111")
                    roleEnum = RoleEnum.Admin;
                else
                    roleEnum = RoleEnum.Employee;

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Email, employee.UserNumber),
                    new Claim(ClaimTypes.Name, employee1?[0].FirstName + " " + employee1?[0].LastName),
                    new Claim(ClaimTypes.Role, roleEnum.ToString())
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
                return RedirectToAction("EmployeeLogin", "Account");
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
            if (result == string.Empty)
            {
                ViewBag.ErrorMessage = "API ile iletişim kurulamadı!";
                return View();
            }

            LoginReturnEnum loginReturnType = JsonConvert.DeserializeObject<LoginReturnEnum>(result);

            if (loginReturnType == LoginReturnEnum.Accept)
            {
                ViewBag.Message = "Giriş işlemi başarılı.";

                dockerApiUrl = new ApiUrlBuilder(UrlTypeEnum.api)
                    .AddEntities(EntitiesEnum.Student)
                    .AddRequest(RequestEnum.Get)
                    .AddMethod(MethodEnum.WithNumber)
                    .AddParameter(student.Number)
                    .Build();
                var result2 = await _requestFactory.SendHttpGetRequest(dockerApiUrl);
                if (result2 == string.Empty)
                {
                    ViewBag.ErrorMessage = "API ile iletişim kurulamadı!2";
                    return View();
                }
                List<Student>? student1 = JsonConvert.DeserializeObject<List<Student>>(result2);

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Email, student.Number),
                    new Claim(ClaimTypes.Name, student1?[0].FirstName + " " + student1?[0].LastName),
                    new Claim(ClaimTypes.Role, RoleEnum.Student.ToString())
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




        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("JointLoginPage", "Account");
        }
    }
}

