using System.ComponentModel.DataAnnotations;

namespace Piolax_WebApp.Models
{
    public class Turnos
    {
        [Key]
        public int idTurno { get; set; }

        [Required]
        [StringLength(30)]
        public string descripcion { get; set; }

        [Required]
        public TimeOnly horaInicio { get; set; }

        [Required]
        public TimeOnly horaFinal { get; set; }

        public ICollection<Solicitudes> Solicitudes { get; set; } // Lista de solicitudes asociados a este turno
    }
}
