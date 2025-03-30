using Microsoft.AspNetCore.Mvc;
using System.Data;
using WebAppCS.Data;

namespace WebAppCS.Controllers
{
    public class UsersController : Controller
    {

        private readonly Database _database;

        public UsersController(Database database)
        {
            _database = database;
        }

        public IActionResult Index()
        {
            string query = "SELECT * FROM Usuarios";
            DataTable usuarios = _database.EjecutarConsulta(query);
            return View("~/Views/Users/Index.cshtml", usuarios);
        }

        public IActionResult Create()
        {
            return View("Create");
        }


        public IActionResult Edit()
        {
            return View("Edit");
        }

        
    }
}
