using System.ComponentModel.DataAnnotations;

namespace Piolax_WebApp.Models
{
    public class EstatusAprobacionMPProduccion
    {
        [Key]
        public int idEstatusAprobacionMPProduccion { get; set; }
        public string descripcionEstatusAprobacionProduccion { get; set; }

        public virtual ICollection<HistoricoMP> HistoricoMP { get; set; } = new List<HistoricoMP>(); // Lista de historicoMP asociados a EstatusAprobacionMPProduccion
    }
}
