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
            // Validar el modelo
            if (!ModelState.IsValid)
            {
                return View(model);
            }

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
    }
}
