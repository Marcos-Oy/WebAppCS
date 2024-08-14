using Microsoft.AspNetCore.Mvc;

namespace WebAppCS.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }
    }
}
