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

        public virtual ICollection<Asignaciones> Asignaciones { get; set; } = new List<Asignaciones>(); // Lista de Asignaciones asociados a StatusAprobacionTecnico
    }
}
