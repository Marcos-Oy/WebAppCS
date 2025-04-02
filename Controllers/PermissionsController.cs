using Microsoft.AspNetCore.Mvc;
using System.Data;
using WebAppCS.Data;
using WebAppCS.Models;

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

            return View("~/Views/Permissions/Index.cshtml");
        }

        [HttpGet]
        public IActionResult PermissionsRole(int id)
        {
            string roles_query = $"SELECT nombre FROM roles where id='{id}'";
            string permisos_query = $"select m.nombre as modulo, p.acceso, p.crear, p.editar, p.eliminar, p.desactivar from permisos p join modulos m on p.id_modulo = m.id where id_rol = '{id}'";
            
            DataTable roles = _database.EjecutarConsulta(roles_query);
            DataTable permisos = _database.EjecutarConsulta(permisos_query);
            
            ViewBag.Permisos = permisos;
            ViewBag.Roles = roles;

            return View("~/Views/Permissions/Permissions.cshtml");
        }

        
    }
}
