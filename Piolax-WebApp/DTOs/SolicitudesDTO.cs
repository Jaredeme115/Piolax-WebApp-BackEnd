using Piolax_WebApp.Models;
using System.ComponentModel.DataAnnotations;

namespace Piolax_WebApp.DTOs
{
    public class SolicitudesDTO
    {
        [Required]
        public string descripcion { get; set; }

        [Required]
        public DateTime fechaSolicitud { get; set; }

        [Required]
        public string numNomina { get; set; }

        [Required]
        public int idMaquina { get; set; }

        [Required]
        public int idTurno { get; set; }

        [Required]
        public int idStatusOrden { get; set; }
        
        [Required]
        public int idStatusAprobacionSolicitante { get; set; }

        [Required]
        public int idAreaSeleccionada { get; set; }

        [Required]
        public int idRolSeleccionado { get; set; }

        [Required]
        public string paroMaquina { get; set; }


    }
}
