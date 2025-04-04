using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Piolax_WebApp.Models
{
    public class KpisMantenimiento
    {
        [Key]
        public int idKPIMantenimiento { get; set; }

        [Required]
        public int idEmpleado { get; set; }
        public Empleado Empleado { get; set; }

        [Required]
        public int idMaquina { get; set; }
        public Maquinas Maquina { get; set; }

        [Required]
        public int idArea { get; set; }
        public Areas Area { get; set; }

        [Required]
        public DateTime fechaCalculo { get; set; }

        public float MTTA { get; set; }
        public float MTTR { get; set; }
        public float MTBF { get; set; }
        public float cumplimientoPlan { get; set; }
        public float efectividadPlan { get; set; }

        public virtual ICollection<KpisDetalle> KpisDetalle { get; set; } = new List<KpisDetalle>();
    }
}
