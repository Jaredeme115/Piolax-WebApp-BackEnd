using System.ComponentModel.DataAnnotations;

namespace Piolax_WebApp.Models
{
    public class EtapaActividad
    {
        [Key]
        public int idActividad { get; set; }

        [Required]
        public int idEtapa { get; set; }

        [Required, MaxLength(255)]
        public string descripcion { get; set; } = null!;

        public int orden { get; set; } = 0;
        public bool completada { get; set; } = false;
        public DateTime? fechaCompletada { get; set; }
        public int? idEmpleadoMarca { get; set; } = 6;

        // Navigation
        public ProyectoEtapa Etapa { get; set; } = null!;
        public Empleado? EmpleadoMarca { get; set; } 
    }
}
