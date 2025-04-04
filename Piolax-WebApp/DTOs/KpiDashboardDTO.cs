namespace Piolax_WebApp.DTOs
{
    public class KpiDashboardDTO
    {
        public string Area { get; set; }
        public string Maquina { get; set; }
        public DateTime Fecha { get; set; }
        public float MTTA { get; set; }
        public float MTTR { get; set; }
        public float TotalDowntime => MTTA + MTTR; // Se calcula automáticamente
        public float MTBF { get; set; }
    }


}
