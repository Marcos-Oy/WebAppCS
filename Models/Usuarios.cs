using System.ComponentModel.DataAnnotations;

namespace WebAppCS.Models
{
    public class Usuarios
    {
        
        public int Id { get; set; }


        [Required(ErrorMessage = "El campo Rut es obligatorio.")]
        [StringLength(12, ErrorMessage = "El Rut no puede exceder los 12 caracteres.")]
        [RegularExpression(@"^\d{1,2}(\.?\d{3})*-?[0-9Kk]$", ErrorMessage = "El formato del RUT es inválido.")]
        public string Rut { get; set; }


        [Required(ErrorMessage = "El campo Nombre es obligatorio.")]
        [StringLength(50, ErrorMessage = "El Nombre no puede exceder los 50 caracteres.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "El Nombre solo puede contener letras y espacios.")]
        public string Nombre { get; set; }


        [Required(ErrorMessage = "El campo Apellidos es obligatorio.")]
        [StringLength(50, ErrorMessage = "El Apellido no puede exceder los 50 caracteres.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "El Apellido solo puede contener letras y espacios.")]
        public string Apellidos { get; set; }


        [Required(ErrorMessage = "El campo Email es obligatorio.")]
        [EmailAddress(ErrorMessage = "El formato del Email es inválido.")]
        [StringLength(100, ErrorMessage = "El Email no puede exceder los 100 caracteres.")]
        public string Email { get; set; }


        [Required(ErrorMessage = "El campo Teléfono es obligatorio.")]
        [StringLength(20, ErrorMessage = "El Teléfono no puede exceder los 20 caracteres.")]
        [RegularExpression(@"^(\+?(\d{1,4}))?(\s?(\(?\d{1,4}\)?))?[\s\-]?\d{6,10}$", ErrorMessage = "El formato del Teléfono no es válido.")]
        public string Telefono { get; set; }


        [Required(ErrorMessage = "El campo Rol es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "Seleccione un Rol válido.")]
        public int Id_rol { get; set; }


        [Required(ErrorMessage = "El campo Estado es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "Seleccione un Estado válido.")]
        public int Id_estado { get; set; }


        public Usuarios()
        {
            Rut = string.Empty;
            Nombre = string.Empty;
            Apellidos = string.Empty;
            Email = string.Empty;
            Telefono = string.Empty;
        }
        
        public Usuarios(int id, string rut, string nombre, string apellidos, string email, string telefono, int id_rol, int id_estado)
        {
            Id = id;
            Rut = rut;
            Nombre = nombre;
            Apellidos = apellidos;
            Email = email;
            Telefono = telefono;
            Id_rol = id_rol;
            Id_estado = id_estado;
        }
    }
}
