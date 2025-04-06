using System.Text.Json;

namespace WebAppCS.Middleware
{
    public static class AuthServiceExtensions
    {
        private const string PermissionsKey = "_UserPermissions";

        public static void SetUserPermissions(this HttpContext context, Dictionary<string, Dictionary<string, bool>> permisos)
        {
            context.Session.SetString(PermissionsKey, JsonSerializer.Serialize(permisos));
        }

        public static Dictionary<string, Dictionary<string, bool>> GetUserPermissions(this HttpContext context)
        {
            var sessionData = context.Session.GetString(PermissionsKey);
            return sessionData == null 
                ? new Dictionary<string, Dictionary<string, bool>>() 
                : JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, bool>>>(sessionData);
        }
    }
}