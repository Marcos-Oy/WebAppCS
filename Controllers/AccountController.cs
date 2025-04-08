using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using WebAppCS.Data;
using WebAppCS.Models;
using MySql.Data.MySqlClient;

namespace WebAppCS.Controllers
{
    public class AccountController : Controller
    {
        private readonly Database _database;

        public AccountController(Database database)
        {
            _database = database;
        }

        public Usuarios AuthenticateUser(string email, string password)
        {
            string passwordMd5 = CalculateMD5(password);
            return GetUserFromDB(_database, email, passwordMd5);
        }

        private string CalculateMD5(string input)
        {
            using var md5 = MD5.Create();
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
        }

        private Usuarios GetUserFromDB(Database database, string email, string passwordMd5)
        {
            using var conn = database.GetConnection();
            conn.Open();

            string query = @"SELECT u.id, u.rut, u.nombre, u.apellidos, u.email, u.telefono, 
                            u.id_rol, u.id_estado, r.nombre as rol, e.estado
                            FROM usuarios u
                            JOIN estados e ON u.id_estado = e.id 
                            JOIN roles r ON u.id_rol = r.id
                            WHERE email = @Email AND password = @Password 
                            AND e.pertenencia = 'usuarios'
                            LIMIT 1";

            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Email", email);
            cmd.Parameters.AddWithValue("@Password", passwordMd5);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new Usuarios
                {
                    Id = reader.GetInt32("id"),
                    Rut = reader.GetString("rut"),
                    Nombre = reader.GetString("nombre"),
                    Apellidos = reader.GetString("apellidos"),
                    Email = reader.GetString("email"),
                    Telefono = reader.GetString("telefono"),
                    Id_rol = reader.GetInt32("id_rol"),
                    Id_estado = reader.GetInt32("id_estado"),
                    Rol = reader.GetString("rol"),
                    Estado = reader.GetString("estado")
                };
            }
            return null;
        }

        public Dictionary<string, Dictionary<string, bool>> GetUserPermissions(int roleId)
        {
            var permisos = new Dictionary<string, Dictionary<string, bool>>();
            
            try
            {
                using var conn = _database.GetConnection();
                conn.Open();

                string query = @"SELECT p.*, m.nombre as modulo
                                FROM permisos p 
                                JOIN modulos m ON p.id_modulo = m.id 
                                WHERE p.id_rol = @RoleId";

                using var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@RoleId", roleId);

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var modulo = reader.GetString("modulo");
                    permisos[modulo] = new Dictionary<string, bool>
                    {
                        {"acceso", reader.GetInt32("acceso") == 1},
                        {"crear", reader.GetInt32("crear") == 1},
                        {"editar", reader.GetInt32("editar") == 1},
                        {"eliminar", reader.GetInt32("eliminar") == 1},
                        {"activar_desactivar", reader.GetInt32("activar_desactivar") == 1},
                        {"restaurar", reader.GetInt32("restaurar") == 1},
                        {"cambiar_password", reader.GetInt32("cambiar_password") == 1}
                    };
                }
            }
            catch
            {
                // Retorna diccionario vacío si hay error
            }
            
            return permisos;
        }
    }
}