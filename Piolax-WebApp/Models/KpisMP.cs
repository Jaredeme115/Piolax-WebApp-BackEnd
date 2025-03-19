using System.ComponentModel.DataAnnotations;

namespace Piolax_WebApp.Models
{
    public class KpisMP
    {
        [Key]
        public int idKPIMP { get; set; }

        public DateTime fechaCalculo { get; set; }

        public virtual ICollection<KpisMPDetalle> KpisMPDetalle { get; set; } = new List<KpisMPDetalle>(); // Lista de KPI´s de Detalle asociados a KPI´s de Mantenimiento Preventivo

    }
}
