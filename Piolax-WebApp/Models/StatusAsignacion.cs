using System.ComponentModel.DataAnnotations;

namespace Piolax_WebApp.Models
{
    public class StatusAsignacion
    {
        [Key]
        public int idStatusAsignacion { get; set; }

        [Required]
        public string descripcionStatusAsignacion { get; set; }

        public virtual ICollection<Asignaciones> Asignaciones { get; set; } = new List<Asignaciones>(); // Lista de StatusAsignacion asociados a Asignaciones
    }
}
