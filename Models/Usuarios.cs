namespace WebAppCS.Models
{
    public class Usuarios
    {
        public int Id { get; set; }
        public string Rut { get; set; }
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        public int Id_rol { get; set; }
        public int Id_estado { get; set; }
    }
}
