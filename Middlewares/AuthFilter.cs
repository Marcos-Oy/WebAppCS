using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebAppCS.Middleware;

namespace WebAppCS.Filters
{
    public class AuthFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            var controllerName = context.Controller.GetType().Name;
            
            // Controladores que no requieren autenticación
            if (controllerName == "HomeController" || controllerName == "AuthController")
                return;

            if (!AuthService.IsAuthenticated(context.HttpContext))
            {
                context.Result = new RedirectToActionResult("Index", "Auth", null);
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // No se necesita implementación
        }
    }
}