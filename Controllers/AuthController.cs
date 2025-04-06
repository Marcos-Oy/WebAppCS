using Microsoft.AspNetCore.Mvc;
using WebAppCS.Data;
using WebAppCS.Middleware;

namespace WebAppCS.Controllers
{
    [NoCache]
    public class AuthController : Controller
    {
        private readonly Database _database;
        private readonly AccountController _accountController;

        public AuthController(Database database, AccountController accountController)
        {
            _database = database;
            _accountController = accountController;
        }

        public IActionResult Index()
        {
            if (AuthService.IsAuthenticated(HttpContext))
                return RedirectToAction("Index", "Dashboard");
                
            return View("~/Views/Account/Login.cshtml");
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Por favor completa todos los campos.";
                return View("~/Views/Account/Login.cshtml");
            }

            var usuario = _accountController.AuthenticateUser(email, password);
            if (usuario != null)
            {
                var permisos = _accountController.GetUserPermissions(usuario.Id_rol);
                AuthService.Login(HttpContext, usuario, permisos);
                return RedirectToAction("Index", "Dashboard");
            }

            ViewBag.Error = "Correo o contraseña incorrectos.";
            return View("~/Views/Account/Login.cshtml");
        }

        [HttpGet]
        public IActionResult Logout()
        {
            // Limpiar toda la sesión
            HttpContext.Session.Clear();
            
            // Invalidar la caché del navegador
            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";
            
            // Redirigir al login con parámetro para evitar caché
            return RedirectToAction("Index", "Auth");
        }
    }
}