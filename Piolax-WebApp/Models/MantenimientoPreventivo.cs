using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Piolax_WebApp.Models
{
    public class MantenimientoPreventivo
    {
        [Key]
        public int idMP { get; set; }

        //[ForeignKey("Area")]
        public int idArea { get; set; }
        public Areas Area { get; set; }

        //[ForeignKey("Maquina")]
        public int idMaquina { get; set; }
        public Maquinas Maquina { get; set; }

        //[ForeignKey("Empleado")]
        public int idEmpleado { get; set; }
        public Empleado Empleado { get; set; }

        public int semanaPreventivo { get; set; }

        //[ForeignKey("FrecuenciaMP")]
        public int idFrecuenciaPreventivo { get; set; }
        public FrecuenciaMP FrecuenciaMP { get; set; }


        public virtual ICollection<HistoricoMP> HistoricoMP { get; set; } = new List<HistoricoMP>(); // Lista de historicoMP asociados a Mantenimiento Preventivo
        public virtual ICollection<MantenimientoPreventivoPDFs> MantenimientoPreventivoPDFs { get; set; } = new List<MantenimientoPreventivoPDFs>(); // Lista de MantenimientoPreventivoPDFs asociados a Mantenimiento Preventivo



    }
}
