using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using WebAppCS.Data;
using WebAppCS.Models;
using MySql.Data.MySqlClient;

namespace WebAppCS.Controllers
{
    public class AccountController : Controller
    {
        private readonly Database _database;

        public AccountController(Database database)
        {
            _database = database;
        }

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

        // POST: /Account/Login
        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Por favor completa todos los campos.";
                return View("Login");
            }

            // Calcular el hash MD5 de la contraseña
            string passwordMd5 = CalcularMD5(password);

            // Validar el usuario contra la base de datos
            var user = ObtenerUsuario(email, passwordMd5);

            if (user != null)
            {
                // Guardar el nombre del usuario en la sesión
                HttpContext.Session.SetString("usuario", user.Nombre);
                return RedirectToAction("Index", "Dashboard");
            }

            ViewBag.Error = "Correo o contraseña incorrectos.";
            return View("Login");
        }

        private Usuarios? ObtenerUsuario(string email, string passwordMd5)
        {
            using var conn = _database.GetConnection();
            conn.Open();

            string query = "SELECT * FROM usuarios WHERE email = @Email AND password = @Password LIMIT 1";
            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Email", email);
            cmd.Parameters.AddWithValue("@Password", passwordMd5);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new Usuarios
                {
                    Id = Convert.ToInt32(reader["id"]),
                    Rut = reader["rut"].ToString(),
                    Nombre = reader["nombre"].ToString(),
                    Apellidos = reader["apellidos"].ToString(),
                    Email = reader["email"].ToString(),
                    Telefono = reader["telefono"].ToString(),
                    Id_rol = Convert.ToInt32(reader["id_rol"]),
                    Id_estado = Convert.ToInt32(reader["id_estado"])
                };
            }

            return null;
        }

        private string CalcularMD5(string input)
        {
            using var md5 = MD5.Create();
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
        }
    }
}
