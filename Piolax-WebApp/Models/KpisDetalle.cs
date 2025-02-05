using System.ComponentModel.DataAnnotations;

namespace Piolax_WebApp.Models
{
    public class KpisDetalle
    {
        [Key]
        public int idKPIDetalle { get; set; }

        [Required]
        public int idKPIMantenimiento { get; set; }

        public KpisMantenimiento KpisMantenimiento { get; set; }

        [Required]
        public float kpiValor { get; set; }
    }
}
