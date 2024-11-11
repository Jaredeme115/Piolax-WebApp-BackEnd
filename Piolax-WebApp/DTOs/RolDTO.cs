using System.ComponentModel.DataAnnotations;

namespace Piolax_WebApp.DTOs
{
    public class RolDTO
    {
        [Required(ErrorMessage = "El Rol es requerido")]
        public string nombreRol { get; set; }
    }
}
