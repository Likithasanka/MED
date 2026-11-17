using MED.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace MED.Controllers
{
    public class HomeController : Controller
    {
        // Home Page
        public IActionResult Index()
        {
            return View();
        }

        // About Page
        public IActionResult About()
        {
            return View();
        }

        // Features Page
        public IActionResult Features()
        {
            return View();
        }

        // Contact Page
        public IActionResult Contact()
        {
            return View();
        }

        // Login Page
        public IActionResult Login()
        {
            return RedirectToAction("Login", "Account");
        }

        // Privacy Page
        public IActionResult Privacy()
        {
            return View();
        }

        // Error Page
        [ResponseCache(
            Duration = 0,
            Location = ResponseCacheLocation.None,
            NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id
                    ?? HttpContext.TraceIdentifier
            });
        }
    }
}