using AnonimousMailServiceTest.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AnonimousMailServiceTest.Controllers
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

        [HttpGet]
        public IActionResult UserPage(string userName)
        {
            ViewBag.UserName = userName;
            return View();
        }

        public void Send(string name, string message)
        {
            // Call the addNewMessageToPage method to update clients.
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