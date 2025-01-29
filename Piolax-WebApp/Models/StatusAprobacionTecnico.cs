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

        public virtual ICollection<Asignacion_Tecnico> Asignacion_Tecnicos { get; set; } = new List<Asignacion_Tecnico>(); // Lista de StatusAprobacionTecnico asociados a Asignacion_Tecnico
    }
}
