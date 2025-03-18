using Piolax_WebApp.Models;
using System.ComponentModel.DataAnnotations;

namespace Piolax_WebApp.DTOs
{
    public class MantenimientoPreventivoRefaccionesDTO
    {

        public int idMP { get; set; }

        public int idRefaccion { get; set; }

        public int cantidad { get; set; }

        public DateTime? fechaUso { get; set; } = DateTime.UtcNow; // Usa fecha actual si no se especifica

    }
}
