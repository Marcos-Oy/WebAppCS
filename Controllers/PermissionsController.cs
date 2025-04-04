using Microsoft.AspNetCore.Mvc;
using System.Data;
using WebAppCS.Data;
using WebAppCS.Models;
using System.Security.Cryptography;
using System.Text;

namespace WebAppCS.Controllers
{
    public class PermissionsController : Controller
    {
        private readonly Database _database;

        public PermissionsController(Database database)
        {
            _database = database;
        }

        public IActionResult Index()
        {
            string query = "SELECT * FROM roles";
            DataTable roles = _database.EjecutarConsulta(query);
            ViewBag.Roles = roles;
            ViewBag.RoleModel = new Roles();
            return View("~/Views/Permissions/Index.cshtml");
        }

        [HttpPost]
        public IActionResult Insert(Roles model)
        {
            // Generar un hash único para esta inserción
            string rawData = model.Nombre + DateTime.Now.Ticks;
            string hash = GenerateSha256Hash(rawData);

            // Insertar el rol con el hash
            string insertQuery = $"INSERT INTO Roles (Nombre, Hash) VALUES ('{model.Nombre}', '{hash}')";
            _database.EjecutarComando(insertQuery);

            // Buscar el ID usando el hash
            string selectQuery = $"SELECT id FROM Roles WHERE Hash = '{hash}'";
            DataTable resultTable = _database.EjecutarConsulta(selectQuery) as DataTable;

            if (resultTable != null && resultTable.Rows.Count > 0)
            {
                int insertedId = Convert.ToInt32(resultTable.Rows[0][0]);

                string moduloUsuario = $"INSERT INTO Permisos (id_rol, id_modulo) VALUES('{insertedId}', 1)";
                _database.EjecutarComando(moduloUsuario);

                string moduloRoles = $"INSERT INTO Permisos (id_rol, id_modulo) VALUES('{insertedId}', 2)";
                _database.EjecutarComando(moduloRoles);

                string moduloPermisos = $"INSERT INTO Permisos (id_rol, id_modulo) VALUES('{insertedId}', 3)";
                _database.EjecutarComando(moduloPermisos);
            }

            return RedirectToAction("Index", "Permissions");
        }

        [HttpPost]
        public IActionResult UpdateRole(Roles model)
        {
            if (!ModelState.IsValid) // Validar el modelo antes de proceder
            {
                return View("Index", model); // Regresa a la vista con los errores de validación
            }
            // Asegurar que el nombre esté entre comillas simples para evitar errores SQL
            string query = $"UPDATE Roles SET nombre = '{model.Nombre}' WHERE Id = {model.Id}";
            _database.EjecutarComando(query);

            return RedirectToAction("Index", "Permissions");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            // Consulta para contar usuarios con este rol
            string queryUsers = $"SELECT COUNT(id) FROM Usuarios WHERE Id_rol = {id}";
            DataTable result = _database.EjecutarConsulta(queryUsers);
            int userCount = Convert.ToInt32(result.Rows[0][0]);
            
            if(userCount >= 1)
            {
                // Si hay usuarios asociados, no se puede eliminar el rol
                TempData["Error"] = "No se puede eliminar el rol porque hay usuarios asociados a él.";
            }
            else
            {
                // Si no hay usuarios asociados, proceder a eliminar el rol y sus permisos
                string queryRole = $"DELETE FROM Roles WHERE Id = {id}";
                _database.EjecutarComando(queryRole);

                string queryPermission = $"DELETE FROM Permisos WHERE id_rol = {id}";
                _database.EjecutarComando(queryPermission);
            }

            return RedirectToAction("Index", "Permissions");
        }

        [HttpGet]
        public IActionResult PermissionsRole(int id)
        {
            string roles_query = $"SELECT nombre FROM roles where id='{id}'";
            // Asegúrate de seleccionar p.id como "id" (sin alias)
            string permisos_query = $"SELECT p.id, m.nombre as modulo, p.acceso, p.crear, p.editar, p.eliminar, p.desactivar FROM permisos p JOIN modulos m ON p.id_modulo = m.id WHERE id_rol = '{id}'";
            
            DataTable roles = _database.EjecutarConsulta(roles_query);
            DataTable permisos = _database.EjecutarConsulta(permisos_query);
            
            ViewBag.Permisos = permisos;
            ViewBag.Roles = roles;

            return View("~/Views/Permissions/Permissions.cshtml");
        }

        [HttpPost]
        public IActionResult UpdatePermissions(Dictionary<string, Permisos> permisos)
        {
            if (permisos == null || permisos.Count == 0)
            {
                Console.WriteLine("Error: No se recibieron permisos");
                return RedirectToAction("Index");
            }

            foreach (var item in permisos)
            {
                var modulo = item.Key;
                var permiso = item.Value;

                // Validación CRÍTICA para el ID
                if (permiso.Id <= 0)
                {
                    Console.WriteLine($"ERROR: ID inválido ({permiso.Id}) para módulo {modulo}");
                    continue; // Saltar este módulo
                }

                Console.WriteLine($"Actualizando módulo {modulo} (ID: {permiso.Id})");

                string query = $@"
                    UPDATE permisos 
                    SET acceso = {(permiso.Acceso ? 1 : 0)},
                        crear = {(permiso.Crear ? 1 : 0)},
                        editar = {(permiso.Editar ? 1 : 0)},
                        eliminar = {(permiso.Eliminar ? 1 : 0)},
                        desactivar = {(permiso.Desactivar ? 1 : 0)}
                    WHERE id = {permiso.Id}";

                try
                {
                    _database.EjecutarComando(query);
                    Console.WriteLine($"✅ Módulo {modulo} actualizado");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Error en {modulo}: {ex.Message}");
                }
            }

            return RedirectToAction("Index");
        }

        // Método para generar SHA256
        private string GenerateSha256Hash(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        
    }
}
