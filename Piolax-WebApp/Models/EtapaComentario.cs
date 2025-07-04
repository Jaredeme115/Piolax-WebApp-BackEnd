using System.ComponentModel.DataAnnotations;

namespace Piolax_WebApp.Models
{
    public class EtapaComentario
    {
        [Key]
        public int idComentario { get; set; }

        [Required]
        public int idEtapa { get; set; }

        [Required]
        public int idEmpleado { get; set; }

        [Required]
        public string textoComentario { get; set; } = null!;

        public DateTime fechaComentario { get; set; } = DateTime.Now;

        // Navigation
        public ProyectoEtapa Etapa { get; set; } = null!;
        public Empleado Empleado { get; set; } = null!;
    }
}
