namespace Piolax_WebApp.DTOs
{
    public class KpiDashboardDTO
    {
        public float mtta { get; set; }
        public float mttr { get; set; }
        public float mtbf { get; set; }
        public float totalDowntime { get; set; }
        public DateTime fechaCalculo { get; set; }
    }

}
