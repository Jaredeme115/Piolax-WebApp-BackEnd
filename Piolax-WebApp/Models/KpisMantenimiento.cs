using System.ComponentModel.DataAnnotations;

namespace Piolax_WebApp.Models
{
    public class KpisMantenimiento
    {
        [Key]
        public int idKPIMantenimiento { get; set; }

        [Required]
        public int? idEmpleado { get; set; }

        public Empleado Empleado { get; set; }

        [Required]
        public int idMaquina { get; set; }
        public Maquinas Maquina { get; set; }

        [Required]
        public int idArea { get; set; }
        public Areas Area { get; set; }

        [Required]
        public DateTime fechaCalculo { get; set; }

        public virtual ICollection<KpisDetalle> KpisDetalle { get; set; } = new List<KpisDetalle>(); // Lista de KPI´s de Detalle asociados a KPI´s de Mantenimiento
    }
}
