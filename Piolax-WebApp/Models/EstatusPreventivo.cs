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

        public virtual ICollection<HistoricoMP> HistoricoMP { get; set; } = new List<HistoricoMP>(); // Lista de HistoricosMPs asociados a EstatusPreventivo
    }
}
