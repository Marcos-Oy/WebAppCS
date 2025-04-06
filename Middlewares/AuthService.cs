using System.Text.Json;
using WebAppCS.Models;

namespace WebAppCS.Middleware
{
    public static class AuthService
    {
        private const string SessionKey = "_UserAuthData";
        private const string PermissionsKey = "_UserPermissions";

        public static void Login(HttpContext context, Usuarios usuario, Dictionary<string, Dictionary<string, bool>> permisos)
        {
            context.Session.SetString(SessionKey, JsonSerializer.Serialize(usuario));
            context.Session.SetString(PermissionsKey, JsonSerializer.Serialize(permisos));
        }

        public static Usuarios GetCurrentUser(HttpContext context)
        {
            var sessionData = context.Session.GetString(SessionKey);
            return sessionData == null ? null : JsonSerializer.Deserialize<Usuarios>(sessionData);
        }

        public static Dictionary<string, Dictionary<string, bool>> GetCurrentPermissions(HttpContext context)
        {
            var sessionData = context.Session.GetString(PermissionsKey);
            return sessionData == null 
                ? new Dictionary<string, Dictionary<string, bool>>() 
                : JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, bool>>>(sessionData);
        }

        public static void Logout(HttpContext context)
        {
            context.Session.Remove(SessionKey);
            context.Session.Remove(PermissionsKey);
        }

        public static bool IsAuthenticated(HttpContext context)
        {
            return GetCurrentUser(context) != null;
        }
    }
}