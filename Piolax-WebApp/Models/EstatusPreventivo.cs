using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Piolax_WebApp.Models
{
    public class EstatusPreventivo
    {
        [Key]
        public int idEstatusPreventivo { get; set; }
        public string nombreEstatusPreventivo { get; set; }
        public string color { get; set; }

        public virtual ICollection<MantenimientoPreventivo> MantenimientosPreventivos { get; set; } = new List<MantenimientoPreventivo>(); // Lista de MAntenimientos Preventivos asociados a EstatusPreventivo
    }
}
