using System.ComponentModel.DataAnnotations;

namespace Piolax_WebApp.Models
{
    public class ProyectoEtapa
    {
        [Key]
        public int idEtapa { get; set; }

        [Required]
        public int idProyecto { get; set; }

        [Required]
        public byte nivelMR { get; set; }

        [Required]
        public byte porcentaje { get; set; } = 0;

        public DateTime fechaUltActualiza { get; set; } = DateTime.Now;

        // Navigation
        public Proyecto Proyecto { get; set; } = null!;
        public ICollection<EtapaActividad> EtapaActividades { get; set; } = new List<EtapaActividad>();
        public ICollection<EtapaComentario> EtapaComentarios { get; set; } = new List<EtapaComentario>();
    }
}

