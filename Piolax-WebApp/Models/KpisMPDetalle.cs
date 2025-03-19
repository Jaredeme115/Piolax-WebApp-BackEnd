namespace Piolax_WebApp.Models
{
    public class KpisMPDetalle
    {
        public int idKPIMPDetalle { get; set; }

        public int idKPIMP { get; set; }
        public KpisMP KpisMP { get; set; }

        public string kpiMPNombre { get; set; }

        public float kpiMPValor { get; set; }
    }
}
