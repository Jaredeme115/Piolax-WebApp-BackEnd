using System.ComponentModel.DataAnnotations;

namespace Piolax_WebApp.Models
{
    public class Firmas
    {
        [Key]
        public int idFirma { get; set; }
        public int idEmpleado { get; set; }
        public Empleado Empleado { get; set; }

        public int idTipoFirma { get; set; }
        public TipoFirmas TipoFirma { get; set; }

        public DateTime fechaFirma { get; set; } = DateTime.UtcNow;

        public virtual ICollection<HistoricoMPFirma> HistoricoMPFirma { get; set; } = new List<HistoricoMPFirma>(); // Lista de HistoricoMPFirma asociados a Firmas
    }
}
