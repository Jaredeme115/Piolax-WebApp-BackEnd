using System.ComponentModel.DataAnnotations;

namespace Piolax_WebApp.Models
{
    public class StatusAprobacionSolicitante
    {
        [Key]
        public int idStatusAprobacionSolicitante { get; set; }

        [Required]
        [StringLength(30)]
        public string descripcionStatusAprobacionSolicitante { get; set; }

        public ICollection<Solicitudes> Solicitudes { get; set; } // Lista de solicitudes asociados a statusOrdenAprobacionSolicitante
    }
}
