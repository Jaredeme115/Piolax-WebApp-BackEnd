using System.ComponentModel.DataAnnotations;

namespace Piolax_WebApp.Models
{
    public class Areas
    {
        [Key]
        public int idArea { get; set; }

        [Required]
        public string nombreArea { get; set; }

        public ICollection<Maquinas> Maquinas { get; set; } // Lista de maquinas asociados a esta area

        public ICollection<EmpleadoAreaRol> EmpleadoAreaRol { get; set; } // Lista de empleadoAreaRol asociados a esta area
    }
}
