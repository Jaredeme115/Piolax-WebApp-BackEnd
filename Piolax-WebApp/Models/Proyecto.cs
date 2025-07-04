using System.ComponentModel.DataAnnotations;

namespace Piolax_WebApp.Models
{
    public class Proyecto
    {
        [Key]
        public int idProyecto { get; set; }

        [Required, MaxLength(50)]
        public string proyectoNo { get; set; } = null!;

        [Required, MaxLength(100)]
        public string nombreProyecto { get; set; } = null!;

        [MaxLength(100)]
        public string? cliente { get; set; }

        public int? idKeyPerson { get; set; }

        [Required]
        public string statusProyecto { get; set; } = "Open";

        public DateTime fechaCreacion { get; set; } = DateTime.Now;

        public DateTime? fechaCierre { get; set; }

        // Navigation
        public Empleado? KeyPerson { get; set; }
        public ICollection<ProyectoEtapa> ProyectoEtapas { get; set; } = new List<ProyectoEtapa>();
        public ICollection<ProyectoFirma> ProyectoFirmas { get; set; } = new List<ProyectoFirma>();
    }
}
