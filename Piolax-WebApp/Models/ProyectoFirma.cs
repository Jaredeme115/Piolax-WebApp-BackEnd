using System.ComponentModel.DataAnnotations;

namespace Piolax_WebApp.Models
{
    public class ProyectoFirma
    {
        [Key]
        public int idFirma { get; set; }

        [Required]
        public int idProyecto { get; set; }

        [Required]
        public int idEmpleado { get; set; }

        [Required, MaxLength(100)]
        public string nombreCompleto { get; set; } = null!;

        public DateTime fechaFirma { get; set; } = DateTime.Now;

        // Navigation
        public Proyecto Proyecto { get; set; } = null!;
        public Empleado Empleado { get; set; } = null!;
    }
}
