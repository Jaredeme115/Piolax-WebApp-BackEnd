using System.ComponentModel.DataAnnotations;

namespace Piolax_WebApp.Models
{
    public class KpiObjetivos
    {
        [Key]
        public int idKpiObjetivo { get; set; }
        public int idArea { get; set; }
        public Areas Areas { get; set; }
        public int anio { get; set; }
        public int mes { get; set; }
        public float valorHoras { get; set; }
        public DateTime fechaCreacion { get; set; }
        
    }
}
