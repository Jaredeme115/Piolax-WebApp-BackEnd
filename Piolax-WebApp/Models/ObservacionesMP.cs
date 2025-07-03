using System.ComponentModel.DataAnnotations;

namespace Piolax_WebApp.Models
{
    public class ObservacionesMP
    {
        [Key]
        public int idObservacionMP { get; set; }
        public int idHistoricoMP { get; set; }
        public HistoricoMP HistoricoMP { get; set; }

        public string observacion { get; set; }
        public DateTime? fechaObservacion { get; set; } = DateTime.UtcNow;
    }
}
