using Piolax_WebApp.Models;
using System.ComponentModel.DataAnnotations;

namespace Piolax_WebApp.DTOs
{
    public class MantenimientoPreventivoPDFCrearDTO
    {
        public int idMP { get; set; }

        [Required]
        [StringLength(255)]
        public string rutaPDF { get; set; } = string.Empty;
    }
}
