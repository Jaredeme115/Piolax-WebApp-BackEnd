using System.ComponentModel.DataAnnotations;

namespace Piolax_WebApp.Models
{
    public class CategoriaAsignacion
    {
        [Key]
        public int idCategoriaAsignacion { get; set; }

        [Required]
        public string descripcion { get; set; }

        public virtual ICollection<Asignaciones> Asignaciones { get; set; } = new List<Asignaciones>(); // Lista de Asignaciones asociados a CategoriaAsignacion
    }
}
