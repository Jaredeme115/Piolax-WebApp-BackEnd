using System.ComponentModel.DataAnnotations;

namespace Piolax_WebApp.Models
{
    public class Roles
    {
        [Key]
        public int idRol { get; set; }

        [Required]
        public string nombreRol { get; set; }

        public ICollection<EmpleadoAreaRol> EmpleadoAreaRol { get; set; } // Lista de empleadoAreaRol asociados a este rol
    }
}
