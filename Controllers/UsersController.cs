using Microsoft.AspNetCore.Mvc;
using System.Data;
using WebAppCS.Data;
using WebAppCS.Models;
using WebAppCS.Middleware;
using System.Security.Cryptography;
using System.Text;

namespace WebAppCS.Controllers
{
    public class UsersController : Controller
    {
        private readonly Database _database;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UsersController(Database database, IHttpContextAccessor httpContextAccessor)
        {
            _database = database;
            _httpContextAccessor = httpContextAccessor;
        }
        
        #region "Métodos GET"

        [HttpGet]
        public IActionResult Index()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var usuario = AuthService.GetCurrentUser(httpContext);
            var permisos = AuthService.GetCurrentPermissions(httpContext);

            string query = string.Empty;

            if(permisos["usuarios"]["restaurar"] == false)
            {
                query = @"SELECT u.id, u.rut, u.apellidos, u.nombre, u.email, u.telefono, e.estado, r.nombre as rol
                FROM Usuarios u
                JOIN Roles r ON u.id_rol = r.id
                JOIN Estados e ON u.id_estado = e.id
                WHERE e.pertenencia = 'usuarios'
                AND e.estado != 'Eliminado'
                AND r.nombre != 'root'
                ORDER BY u.id DESC";
            }

            if(permisos["usuarios"]["restaurar"] == true)
            {
                query = @"SELECT u.id, u.rut, u.apellidos, u.nombre, u.email, u.telefono, e.estado, r.nombre as rol
                FROM Usuarios u
                JOIN Roles r ON u.id_rol = r.id
                JOIN Estados e ON u.id_estado = e.id
                WHERE e.pertenencia = 'usuarios'
                AND r.nombre != 'root'
                ORDER BY u.id DESC";
            }
            
            DataTable usuarios = _database.EjecutarConsulta(query);
            return View("~/Views/Users/Index.cshtml", usuarios); // Pasar la lista de usuarios a la vista
        }
        
        [HttpGet]
        public IActionResult Create()
        {
            string queryEstados = @" SELECT e.id, e.estado
            FROM Estados e
            WHERE e.pertenencia = 'usuarios'
            AND e.estado != 'Eliminado'";

            string queryRoles = @" SELECT r.id, r.nombre, r.hash
            FROM Roles r WHERE r.nombre != 'root'";
            
            DataTable estados = _database.EjecutarConsulta(queryEstados);
            DataTable roles = _database.EjecutarConsulta(queryRoles);

            ViewBag.Estados = estados;
            ViewBag.Roles = roles;

            return View("~/Views/Users/Create.cshtml");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            // Obtener el usuario por su ID
            string queryUsuario = $"SELECT * FROM Usuarios WHERE Id = {id}";
            DataTable dataTable = _database.EjecutarConsulta(queryUsuario);
            
            if (dataTable.Rows.Count == 0)
            {
                return NotFound(); // Si no se encuentra el usuario, mostrar error 404
            }

            // Mapear la fila de datos a un objeto de tipo 'Usuarios'
            var usuario = new Usuarios(
                id: dataTable.Rows.Cast<DataRow>().FirstOrDefault()?["Id"] as int? ?? 0,
                rut: dataTable.Rows.Cast<DataRow>().FirstOrDefault()?["Rut"]?.ToString() ?? string.Empty,
                nombre: dataTable.Rows.Cast<DataRow>().FirstOrDefault()?["Nombre"]?.ToString() ?? string.Empty,
                apellidos: dataTable.Rows.Cast<DataRow>().FirstOrDefault()?["Apellidos"]?.ToString() ?? string.Empty,
                email: dataTable.Rows.Cast<DataRow>().FirstOrDefault()?["Email"]?.ToString() ?? string.Empty,
                telefono: dataTable.Rows.Cast<DataRow>().FirstOrDefault()?["Telefono"]?.ToString() ?? string.Empty,
                id_rol: dataTable.Rows.Cast<DataRow>().FirstOrDefault()?["Id_rol"] as int? ?? 0,
                id_estado: dataTable.Rows.Cast<DataRow>().FirstOrDefault()?["Id_estado"] as int? ?? 0
            );

            string queryEstados = @" SELECT e.id, e.estado
            FROM Estados e
            WHERE e.pertenencia = 'usuarios'
            AND e.estado != 'Eliminado'";

            string queryRoles = @" SELECT r.id, r.nombre, r.hash
            FROM Roles r WHERE r.nombre != 'root'";
            
            DataTable estados = _database.EjecutarConsulta(queryEstados);
            DataTable roles = _database.EjecutarConsulta(queryRoles);

            ViewBag.Estados = estados;
            ViewBag.Roles = roles;

            return View("~/Views/Users/Edit.cshtml", usuario);
        }

        #endregion
        
        #region "Métodos POST"
        
        [HttpPost]
        public IActionResult Insert(Usuarios model)
        {
            ModelState.Remove("Rol");
            ModelState.Remove("Estado");

            if (!ModelState.IsValid)
            {
                // 🔍 Muestra los errores de validación en la consola/logs
                foreach (var key in ModelState.Keys)
                {
                    var errors = ModelState[key].Errors;
                    foreach (var error in errors)
                    {
                        Console.WriteLine($"Error en {key}: {error.ErrorMessage}");
                    }
                }

                return RedirectToAction("Create");
            }

            if (!ValidarRut(model.Rut))
            {
                // Guardar el error en TempData
                TempData["RutError"] = "El RUT ingresado no es válido.";
                return RedirectToAction("Create");
            }

            // Formatear el RUT antes de guardarlo
            model.Rut = FormatearRut(model.Rut);
            // Formatear el TELÉFONO antes de guardarlo
            model.Telefono = model.Telefono.Replace(" ", string.Empty);
            // Formatear el CORREO antes de guardarlo
            model.Email = model.Email.Replace(" ", "").ToLower();
            // Generar el hash MD5 de la contraseña
            string passwordMD5 = GenerarMD5(model.Password);

            // Consulta SQL con interpolación
            string query = $"INSERT INTO Usuarios (Rut, Nombre, Apellidos, Email, Telefono, Id_rol, Id_estado, Password) " +
                        $"VALUES ('{model.Rut}', '{model.Nombre}', '{model.Apellidos}', '{model.Email}', '{model.Telefono}', {model.Id_rol}, {model.Id_estado}, '{passwordMD5}')";
            _database.EjecutarComando(query);

            TempData["ToastrMessage"] = "Usuario creado correctamente";
            TempData["ToastrType"] = "success"; // success | info | warning | error 

            return RedirectToAction("Index", "Users");
        }

        [HttpPost]
        public IActionResult UpdateUser(Usuarios model)
        {
            // Eliminar la contraseña de ModelState para evitar que se valide
            ModelState.Remove("Password");
            ModelState.Remove("RepeatPassword");
            ModelState.Remove("Id_estado");
            ModelState.Remove("Id_rol");
            ModelState.Remove("Rol");
            ModelState.Remove("Estado");

            if (!ModelState.IsValid)
            {
                // 🔍 Muestra los errores de validación en la consola/logs
                foreach (var key in ModelState.Keys)
                {
                    var errors = ModelState[key].Errors;
                    foreach (var error in errors)
                    {
                        Console.WriteLine($"Error en {key}: {error.ErrorMessage}");
                    }
                }

                return RedirectToAction("Edit", new { id = model.Id });
            }
            
            if (!ValidarRut(model.Rut))
            {
                // Guardar el error en TempData
                TempData["RutError"] = "El RUT ingresado no es válido.";
                return RedirectToAction("Edit", new { id = model.Id });
            }

            // Formatear el RUT antes de guardarlo
            model.Rut = FormatearRut(model.Rut);
            // Formatear el TELÉFONO antes de guardarlo
            model.Telefono = model.Telefono.Replace(" ", string.Empty);
            // Formatear el EMAIL antes de guardarlo
            model.Email = model.Email.Replace(" ", "").ToLower();

            // Lógica para actualizar el usuario
            string query = $"UPDATE Usuarios SET Rut = '{model.Rut}', Nombre = '{model.Nombre}', Apellidos = '{model.Apellidos}', Email = '{model.Email}', Telefono = '{model.Telefono}' WHERE Id = {model.Id}";
            _database.EjecutarComando(query);

            TempData["ToastrMessage"] = "Usuario actualizado correctamente";
            TempData["ToastrType"] = "success"; // success | info | warning | error 

            return RedirectToAction("Index", "Users");
        }

        [HttpPost]
        public IActionResult UpdateEstado(Usuarios model)
        {
            // Eliminar la contraseña de ModelState para evitar que se valide
            ModelState.Remove("Rut");
            ModelState.Remove("Nombre");
            ModelState.Remove("Apellidos");
            ModelState.Remove("Email"); 
            ModelState.Remove("Telefono");
            ModelState.Remove("Id_rol");
            ModelState.Remove("Rol");
            ModelState.Remove("Estado");
            ModelState.Remove("Password");
            ModelState.Remove("RepeatPassword");

            if (!ModelState.IsValid)
            {
                // 🔍 Muestra los errores de validación en la consola/logs
                foreach (var key in ModelState.Keys)
                {
                    var errors = ModelState[key].Errors;
                    foreach (var error in errors)
                    {
                        Console.WriteLine($"Error en {key}: {error.ErrorMessage}");
                    }
                }

                return RedirectToAction("Edit", new { id = model.Id });
            }

            // Lógica para actualizar el usuario
            string query = $"UPDATE Usuarios SET Id_estado = {model.Id_estado} WHERE Id = {model.Id}";
            _database.EjecutarComando(query);

            TempData["ToastrMessage"] = "Estado del usuario actualizado correctamente";
            TempData["ToastrType"] = "success"; // success | info | warning | error 

            return RedirectToAction("Index", "Users");
        }

        [HttpPost]
        public IActionResult UpdateRol(Usuarios model)
        {
            // Eliminar la contraseña de ModelState para evitar que se valide
            ModelState.Remove("Rut");
            ModelState.Remove("Nombre");
            ModelState.Remove("Apellidos");
            ModelState.Remove("Email"); 
            ModelState.Remove("Telefono");
            ModelState.Remove("Id_estado");
            ModelState.Remove("Rol");
            ModelState.Remove("Estado");
            ModelState.Remove("Password");
            ModelState.Remove("RepeatPassword");

            if (!ModelState.IsValid)
            {
                // 🔍 Muestra los errores de validación en la consola/logs
                foreach (var key in ModelState.Keys)
                {
                    var errors = ModelState[key].Errors;
                    foreach (var error in errors)
                    {
                        Console.WriteLine($"Error en {key}: {error.ErrorMessage}");
                    }
                }

                return RedirectToAction("Edit", new { id = model.Id });
            }

            // Lógica para actualizar el usuario
            string query = $"UPDATE Usuarios SET Id_rol = {model.Id_rol} WHERE Id = {model.Id}";
            _database.EjecutarComando(query);

            TempData["ToastrMessage"] = "Rol del usuario actualizado correctamente";
            TempData["ToastrType"] = "success"; // success | info | warning | error 

            return RedirectToAction("Index", "Users");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdatePassword(Usuarios model)
        {
            // Eliminar la contraseña de ModelState para evitar que se valide
            ModelState.Remove("Id");
            ModelState.Remove("Rut");
            ModelState.Remove("Nombre");
            ModelState.Remove("Apellidos");
            ModelState.Remove("Email"); 
            ModelState.Remove("Telefono");
            ModelState.Remove("Id_rol");
            ModelState.Remove("Id_estado");
            ModelState.Remove("Rol");
            ModelState.Remove("Estado");
            
            if (!ModelState.IsValid)
            {
                // 🔍 Muestra los errores de validación en la consola/logs
                foreach (var key in ModelState.Keys)
                {
                    var errors = ModelState[key].Errors;
                    foreach (var error in errors)
                    {
                        Console.WriteLine($"Error en {key}: {error.ErrorMessage}");
                    }
                }

                return RedirectToAction("Edit", new { id = model.Id });
            }

            // Generar el hash MD5 de la contraseña
            string passwordMD5 = GenerarMD5(model.Password);

            // Lógica para actualizar el usuario
            string query = $"UPDATE Usuarios SET Password = '{passwordMD5}' WHERE Id = {model.Id}";
            _database.EjecutarComando(query);

            TempData["ToastrMessage"] = "Contraseña actualizada correctamente";
            TempData["ToastrType"] = "success"; // success | info | warning | error 

            return RedirectToAction("Index", "Users");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            // string query = $"DELETE FROM Usuarios WHERE Id = {id}";
            // _database.EjecutarComando(query);

            try{
                string query = $@"UPDATE Usuarios SET 
                Id_estado = (select id from Estados where pertenencia = 'usuarios' and estado = 'Eliminado')
                WHERE Id = {id}";

                _database.EjecutarComando(query);
                
                int filasAfectadas = _database.EjecutarComando(query);
                Console.WriteLine($"Filas afectadas: {filasAfectadas}");

                TempData["ToastrMessage"] = "Usuario eliminado correctamente" + id;
                TempData["ToastrType"] = "success"; // success | info | warning | error 
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                TempData["ToastrMessage"] = "Error al eliminar el usuario: " + ex.Message;
                TempData["ToastrType"] = "error"; // success | info | warning | error 
            }
            catch (Exception ex)
            {
                TempData["ToastrMessage"] = "Error al eliminar el usuario: " + ex.Message;
                TempData["ToastrType"] = "error"; // success | info | warning | error 
            }

            return RedirectToAction("Index", "Users");
        }

        [HttpPost]
        public IActionResult Restore(int id)
        {
            // string query = $"DELETE FROM Usuarios WHERE Id = {id}";
            // _database.EjecutarComando(query);
            try{
                string query = $@"UPDATE Usuarios SET 
                Id_estado = (select id from Estados where pertenencia = 'usuarios' and estado = 'Inactivo')
                WHERE Id = {id}";

                _database.EjecutarComando(query);
                
                int filasAfectadas = _database.EjecutarComando(query);
                Console.WriteLine($"Filas afectadas: {filasAfectadas}");

                TempData["ToastrMessage"] = "Usuario restaurado correctamente" + id;
                TempData["ToastrType"] = "success"; // success | info | warning | error 
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                TempData["ToastrMessage"] = "Error al restaurar el usuario: " + ex.Message;
                TempData["ToastrType"] = "error"; // success | info | warning | error 
            }
            catch (Exception ex)
            {
                TempData["ToastrMessage"] = "Error al restaurar el usuario: " + ex.Message;
                TempData["ToastrType"] = "error"; // success | info | warning | error 
            }

            return RedirectToAction("Index", "Users");
        }

        #endregion

        public static bool ValidarRut(string rut)
        {
            rut = rut.Replace(".", "").Replace("-", "").ToUpper();
            
            if (rut.Length < 2) return false;

            // Verificar que el RUT no sea un valor genérico o inventado
            if (rut == "777777777") return false;  // Ejemplo de un RUT repetitivo o "inventado"
            
            string digitoVerificador = rut[^1].ToString(); // Último carácter
            string rutNumerico = rut[..^1]; // Todo excepto el último carácter

            int suma = 0, multiplicador = 2;
            
            for (int i = rutNumerico.Length - 1; i >= 0; i--)
            {
                suma += (rutNumerico[i] - '0') * multiplicador;
                multiplicador = (multiplicador == 7) ? 2 : multiplicador + 1;
            }

            int resto = suma % 11;
            string digitoCalculado = (11 - resto) switch
            {
                11 => "0",
                10 => "K",
                _ => (11 - resto).ToString()
            };

            return digitoVerificador == digitoCalculado;
        }

        public static string FormatearRut(string rut)
        {
            rut = rut.Replace(".", "").Replace("-", "").ToUpper(); // Eliminar puntos y guion

            if (rut.Length < 2) return rut;

            string rutNumerico = rut.Substring(0, rut.Length - 1); // Todo excepto el último carácter
            string digitoVerificador = rut.Substring(rut.Length - 1); // Último carácter

            // Formatear el rut con puntos
            string rutConPuntos = string.Empty;
            for (int i = rutNumerico.Length; i > 0; i--)
            {
                rutConPuntos = rutNumerico[i - 1] + rutConPuntos;
                if ((rutNumerico.Length - i) % 3 == 2 && i > 1) rutConPuntos = "." + rutConPuntos;
            }

            return rutConPuntos + "-" + digitoVerificador; // Devolver el rut con puntos y guion
        }
    
        private string GenerarMD5(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString().ToLower(); // Para que el hash esté en minúsculas
            }
        }
    }
}
