using EmpresaT3.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace EmpresaT3.Controllers
{
    //[Authorize(Roles = $"{Constants.Roles.Administrator},{Constants.Roles.Manager}")]
    //[Authorize(Policy = Constants.Policies.RequireAdmin)]
    //[Authorize(Policy = "RequireAdmin")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        public IActionResult Login()
        {
            return LocalRedirect("/Identity/Account/Login");
        }

        public IActionResult Index()
        {
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
}