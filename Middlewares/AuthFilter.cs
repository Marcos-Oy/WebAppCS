using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebAppCS.Middleware
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
                // Añadir headers anti-caché
                context.HttpContext.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
                context.HttpContext.Response.Headers["Pragma"] = "no-cache";
                
                context.Result = new RedirectToActionResult("Index", "Auth", null);
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // No se necesita implementación
        }
    }
}