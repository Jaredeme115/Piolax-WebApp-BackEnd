using Piolax_WebApp.Models;
using System.ComponentModel.DataAnnotations;

namespace Piolax_WebApp.DTOs
{
    public class MantenimientoPreventivoPDFsDTO
    {
        public int idMP { get; set; }

        [StringLength(255)]
        public string nombrePDF { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string rutaPDF { get; set; } = string.Empty;
    }
}
