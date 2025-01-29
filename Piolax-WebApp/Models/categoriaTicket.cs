using System.ComponentModel.DataAnnotations;

namespace Piolax_WebApp.Models
{
    public class categoriaTicket
    {
        [Key]
        public int idCategoriaTicket { get; set; }

        [Required]
        public string descripcionCategoriaTicket { get; set; }

        public ICollection<Solicitudes> Solicitudes { get; set; } // Lista de solicitudes asociados a CategoriaTicket
    }
}
