namespace WebAppCS.Models
{
    public class Permisos
    {
        public int Id { get; set; }
        public bool Acceso { get; set; }
        public bool Crear { get; set; }
        public bool Editar { get; set; }
        public bool Eliminar { get; set; }
        public bool Desactivar { get; set; }

    }
}