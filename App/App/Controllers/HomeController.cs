using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using App.Models;
using App.Builders;
using App.Enums;

namespace App.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        // Buradaki url test amaçlı oluşturulmuştur:
        // Buradaki url test amaçlı oluşturulmuştur:
        //string apiUrl = new ApiUrlBuilder(UrlTypeEnum.api)
        //                .AddParameter(213)
        //                .AddEntities(EntitiesEnum.Student)
        //                .AddRequest(RequestEnum.Get)
        //                .AddMethod(MethodEnum.All)
        //                .SetHttpSecurity(true)
        //                .SetPortNumber(213)
        //                .SetServerAddress("213.3422")
        //                .Build();
        //ViewBag.Message = apiUrl;
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

