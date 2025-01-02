using System.ComponentModel.DataAnnotations;

namespace Piolax_WebApp.Models
{
    public class StatusAprobacionTecnico
    {
        [Key]
        public int idStatusAprobacionTecnico { get; set; }

        [Required]
        [StringLength(30)]
        public string descripcionStatusAprobacionTecnico { get; set; }

        //public ICollection<Solicitudes> Solicitudes { get; set; } // Lista de solicitudes asociados a statusOrdenAprobacionSolicitante
    }
}
