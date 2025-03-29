using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebAppCS.Models;

namespace WebAppCS.Controllers
{
    public class AccountController : Controller
    {

        // GET: /Account/Login
        public IActionResult Index()
        {
            return View("Login");
        }
        
        // GET: /Account/ForgotPassword
        public IActionResult ForgotPassword()
        {
            return View("ForgotPassword");
        }

         // GET: /Account/Register
        public IActionResult Register()
        {
                return View("Register");
        }

        [HttpPost] // Simula el submit del formulario
        public IActionResult FakeLogin()
        {
            return RedirectToAction("Index", "Dashboard");
        }
        
    }
}
