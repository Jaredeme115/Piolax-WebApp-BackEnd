using System.ComponentModel.DataAnnotations;

namespace Piolax_WebApp.Models
{
    public class EstatusAprobacionMPMantenimiento
    {
        [Key]
        public int idEstatusAprobacionMPMantenimiento { get; set; }

        public string descripcionEstatusAprobacionMantenimiento { get; set; }

        public virtual ICollection<HistoricoMP> HistoricoMP { get; set; } = new List<HistoricoMP>(); // Lista de historicoMP asociados a EstatusAprobacionMPMantenimiento
    }
}
