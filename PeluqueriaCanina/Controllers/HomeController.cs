using Microsoft.AspNetCore.Mvc;
using PeluqueriaCanina.Models;
using PeluqueriaCanina.Models.Users;
using System.Diagnostics;

namespace PeluqueriaCanina.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
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

        public IActionResult AccesoDenegado()
        {
            return View();
        }
    }
}
