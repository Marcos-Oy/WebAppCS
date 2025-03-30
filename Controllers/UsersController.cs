using Microsoft.AspNetCore.Mvc;
using System.Data;
using WebAppCS.Data;
using WebAppCS.Models;

namespace WebAppCS.Controllers
{
    public class UsersController : Controller
    {
        private readonly Database _database;

        public UsersController(Database database)
        {
            _database = database;
        }

        // Acción GET: Mostrar lista de usuarios
        public IActionResult Index()
        {
            string query = "SELECT * FROM Usuarios";
            DataTable usuarios = _database.EjecutarConsulta(query);
            return View("~/Views/Users/Index.cshtml", usuarios); // Pasar la lista de usuarios a la vista
        }

        // Acción GET: Mostrar formulario para crear un nuevo usuario
        public IActionResult Create()
        {
            return View("~/Views/Users/Create.cshtml");
        }

        // Acción GET: Mostrar formulario para editar un usuario
        [HttpGet]
        public IActionResult Edit(int id)
        {
            // Obtener el usuario por su ID
            string query = $"SELECT * FROM Usuarios WHERE Id = {id}";
            DataTable dataTable = _database.EjecutarConsulta(query);
            
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

            return View("~/Views/Users/Edit.cshtml", usuario);
        }

        // Acción POST: Crear un nuevo usuario
        [HttpPost]
        public IActionResult Insert(string nombre, string email)
        {
            string query = $"INSERT INTO Usuarios (Nombre, Email) VALUES ('{nombre}', '{email}')";
            _database.EjecutarComando(query);
            return RedirectToAction("Index", "Users");
        }
        
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(Usuarios model)
        {
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

            // Lógica para actualizar el usuario
            string query = $"UPDATE Usuarios SET Rut = '{model.Rut}', Nombre = '{model.Nombre}', Apellidos = '{model.Apellidos}', Email = '{model.Email}', Telefono = '{model.Telefono}', Id_rol = {model.Id_rol}, Id_estado = {model.Id_estado} WHERE Id = {model.Id}";
            _database.EjecutarComando(query);

            return RedirectToAction("Index", "Users");
        }

        // Acción: Eliminar un usuario
        public IActionResult Delete(int id)
        {
            string query = $"DELETE FROM Usuarios WHERE Id = {id}";
            _database.EjecutarComando(query);
            return RedirectToAction("Index", "Users");
        }

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

    }
}
