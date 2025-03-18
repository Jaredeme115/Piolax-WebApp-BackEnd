using Piolax_WebApp.Models;
using System.ComponentModel.DataAnnotations;

namespace Piolax_WebApp.DTOs
{
    public class MantenimientoPreventivoRefaccionesDetalleDTO
    {
        public int idMPRefaccion { get; set; }
        public int idMP { get; set; }

        public int idRefaccion { get; set; }

        public string? nombreRefaccion { get; set; }

        public int cantidad { get; set; }
    }
}
