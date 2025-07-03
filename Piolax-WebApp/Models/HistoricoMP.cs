using System.ComponentModel.DataAnnotations;

namespace Piolax_WebApp.Models
{
    public class HistoricoMP
    {
        [Key]
        public int idHistoricoMP { get; set; }

        //[ForeignKey("MantenimientoPreventivo")]
        public int idMP { get; set; }
        public MantenimientoPreventivo MantenimientoPreventivo { get; set; }

        //[ForeignKey("Empleado")]
        public int idEmpleadoRealizo { get; set; }
        public Empleado Empleado { get; set; }

        //[ForeignKey("EstatusPreventivo")]
        public int idEstatusPreventivo { get; set; }
        public EstatusPreventivo EstatusPreventivo { get; set; }

        public bool activo { get; set; } = true;

        public int anioPreventivo { get; set; }

        public DateTime fechaAprobacionMantenimiento { get; set; }

        public DateTime fechaAprobacionProduccion { get; set; }

        public DateTime fechaEjecucion { get; set; }

        //[ForeignKey("EstatusAprobacionMPMantenimiento")]
        public int idEstatusAprobacionMPMantenimiento { get; set; }
        public EstatusAprobacionMPMantenimiento EstatusAprobacionMPMantenimiento { get; set; }

        //[ForeignKey("EstatusAprobacionMPProduccion")]
        public int idEstatusAprobacionMPProduccion { get; set; }
        public EstatusAprobacionMPProduccion EstatusAprobacionMPProduccion { get; set; }

        public int semanaRealizacion { get; set; }

        public virtual ICollection<HistoricoPreventivoPDF> HistoricoPreventivoPDF { get; set; } = new List<HistoricoPreventivoPDF>(); // Lista de HistoricoPreventivoPDFs asociados a HistoricoMP
        public virtual ICollection<ObservacionesMP> ObservacionesMP { get; set; } = new List<ObservacionesMP>(); // Lista de ObservacionesMP asociados a HistoricoMP
        public virtual ICollection<MantenimientoPreventivo_Refacciones> MantenimientoPreventivo_Refacciones { get; set; } = new List<MantenimientoPreventivo_Refacciones>(); // Lista de MantenimientoPreventivo_Refacciones asociados a HistoricoMP    
        public virtual ICollection<HistoricoMPFirma> HistoricoMPFirma { get; set; } = new List<HistoricoMPFirma>(); // Lista de HistoricoMPFirma asociados a HistoricoMP

    }
}
