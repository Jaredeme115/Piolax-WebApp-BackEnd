using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Piolax_WebApp.Models
{
    public class MantenimientoPreventivoEjecuciones
    {
        [Key]
        public int idMPEjecucion { get; set; }

        public int idMP { get; set; }
        public MantenimientoPreventivo? MantenimientoPreventivo { get; set; }

        [Required, MaxLength(255)]
        public string nombrePDF { get; set; } = null!;

        [Required, MaxLength(255)]
        public string rutaPDF { get; set; } = null!;

        public int semanaEjecucion { get; set; }
        public int anioEjecucion { get; set; }

        public DateTime fechaEjecucion { get; set; } = DateTime.Now;
    }
}
