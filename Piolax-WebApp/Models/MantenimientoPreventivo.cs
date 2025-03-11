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

        public bool Activo { get; set; } = true;

        [Required]
        [StringLength(255)]
        public string rutaPDF { get; set; } = string.Empty;
        public DateTime? ultimaEjecucion { get; set; }
        public DateTime? proximaEjecucion { get; set; }
        public DateTime? fechaEjecucion { get; set; }

    }
}
