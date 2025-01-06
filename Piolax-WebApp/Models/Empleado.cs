using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Piolax_WebApp.Models
{
    public class Empleado
    {
        [Key]
        public int idEmpleado { get; set; }

        [Required]
        public string numNomina { get; set; }

        [Required]
        public string nombre { get; set; }

        [Required]
        public string apellidoPaterno { get; set; }

        [Required]
        public string apellidoMaterno { get; set; }

        [Required(ErrorMessage = "El telefono es requerido")]
        [Phone(ErrorMessage = "El teléfono no es válido")]
        public string telefono { get; set; }

        [EmailAddress(ErrorMessage = "El email no es válido")]
        public string email { get; set; }

        [Required]
        public DateOnly fechaIngreso { get; set; }

        [Required]
        public byte[] passwordHasH { get; set; }

        [Required]
        public byte[] passwordSalt { get; set; }

        // Relación con la tabla StatusEmpleado
        public int idStatusEmpleado { get; set; }
        public StatusEmpleado StatusEmpleado { get; set; } // Propiedad de navegación

        public ICollection<EmpleadoAreaRol> EmpleadoAreaRol {get; set; } // Lista de empleadoAreaRol asociados a este empleado

        public ICollection<Solicitudes> Solicitudes { get; set; } // Lista de solicitudes asociados a este empleado

        public ICollection<RefreshTokens> RefreshTokens { get; set; } // Lista de refreshTokens asociados a este empleado

        public virtual ICollection<Asignaciones> Asignaciones { get; set; } = new List<Asignaciones>(); // Lista de Asignaciones asociados a Empleado


    }
}
