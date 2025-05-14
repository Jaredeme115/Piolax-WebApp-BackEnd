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

        //[ForeignKey("EstatusPreventivo")]
        public int idEstatusPreventivo { get; set; }
        public EstatusPreventivo EstatusPreventivo { get; set; }

        public bool activo { get; set; } = true;

        public int semanaOriginalMP { get; set; } // Semana original del mantenimiento preventivo

        public int anioPreventivo { get; set; }

        public DateTime? ultimaEjecucion { get; set; }
        public DateTime? proximaEjecucion { get; set; }
        public DateTime? fechaEjecucion { get; set; }

        public virtual ICollection<MantenimientoPreventivoPDFs> MantenimientoPreventivoPDFs { get; set; } = new List<MantenimientoPreventivoPDFs>(); // Lista de MantenimientoPreventivoPDFs asociados a Mantenimiento Preventivo

        public virtual ICollection<ObservacionesMP> ObservacionesMP { get; set; } = new List<ObservacionesMP>(); // Lista de ObservacionesMP asociados a Mantenimiento Preventivo
        
        public virtual ICollection<MantenimientoPreventivo_Refacciones> MantenimientoPreventivo_Refacciones { get; set; } = new List<MantenimientoPreventivo_Refacciones>(); // Lista de MantenimientoPreventivo_Refacciones asociados a Mantenimiento Preventivo

        public virtual ICollection<MantenimientoPreventivoEjecuciones> MantenimientoPreventivoEjecucion { get; set; } = new List<MantenimientoPreventivoEjecuciones>(); // Lista de MantenimientoPreventivoEjecucion asociados a Mantenimiento Preventivo

    }
}
