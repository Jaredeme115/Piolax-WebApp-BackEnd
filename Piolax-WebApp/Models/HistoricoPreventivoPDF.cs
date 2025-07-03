using System.ComponentModel.DataAnnotations;

namespace Piolax_WebApp.Models
{
    public class HistoricoPreventivoPDF
    {
        [Key]
        public int idHistoricoPreventivoPDF { get; set; }

        public int idHistoricoMP { get; set; }
        public HistoricoMP HistoricoMP { get; set; }

        public string nombrePDF { get; set; }
        
        public string rutaPDF { get; set; }
        
        public DateTime fechaSubida { get; set; }

        public int idEmpleadoSubio { get; set; }
        public Empleado Empleado { get; set; }
    }
}
