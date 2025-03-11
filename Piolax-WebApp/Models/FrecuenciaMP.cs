using System.ComponentModel.DataAnnotations;

namespace Piolax_WebApp.Models
{
    public class FrecuenciaMP
    {
        [Key]
        public int idFrecuenciaPreventivo { get; set; }

        [Required]
        [StringLength(30)]
        public string nombreFrecuenciaMP { get; set; }

        public int intervalo { get; set; }

        public UnidadTiempoEnum unidadTiempo { get; set; }

        public virtual ICollection<MantenimientoPreventivo> MantenimientosPreventivos { get; set; } = new List<MantenimientoPreventivo>(); // Lista de MAntenimientos Preventivos asociados a FrecuenciaMP
    }

    // Definición del ENUM dentro de la clase FrecuenciaMP debido a que no se usa en otro lados
    public enum UnidadTiempoEnum
    {
        Semana,
        Mes,
        Año
    }
}
