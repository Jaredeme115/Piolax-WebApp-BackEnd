using System.ComponentModel.DataAnnotations;

namespace Piolax_WebApp.Models
{
    public class MantenimientoPreventivoPDFs
    {

        [Key]
        public int idMPPDF { get; set; }

        public int idMP { get; set; }
        public MantenimientoPreventivo MantenimientoPreventivo { get; set; }

        [StringLength(255)]
        public string nombrePDF { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string rutaPDF { get; set; } = string.Empty;
    }
}
