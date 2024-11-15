using System.ComponentModel.DataAnnotations;

namespace Piolax_WebApp.Models
{
    public class StatusOrden
    {

        [Key]
        public int idStatusOrden { get; set; }

        [Required]
        [StringLength(30)]
        public string descripcionStatusOrden { get; set; }

        [Required]
        [StringLength(15)]
        public string color { get; set; }

        public ICollection<Solicitudes> Solicitudes { get; set; } // Lista de solicitudes asociados a este statusOrden

    }
}
