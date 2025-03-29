using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebAppCS.Models;

namespace WebAppCS.Controllers
{
    public class UsersController : Controller
    {

        public IActionResult Index()
        {
            return View("Index");
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
