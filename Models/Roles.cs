using System.ComponentModel.DataAnnotations;

namespace WebAppCS.Models
{
    public class Roles
    {
        
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo es obligatorio.")]
        [StringLength(50, ErrorMessage = "El rol no puede exceder los 50 caracteres.")]
        public string Nombre { get; set; }

    }
}