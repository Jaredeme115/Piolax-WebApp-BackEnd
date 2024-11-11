using System.ComponentModel.DataAnnotations;

namespace Piolax_WebApp.Models
{
    public class StatusEmpleado
    {
        [Key]
        public int idStatusEmpleado { get; set; }

        [Required]
        public string descripcionStatusEmpleado { get; set; }

        public ICollection<Empleado> Empleados { get; set; } // Lista de empleados asociados a este status

    }
}
