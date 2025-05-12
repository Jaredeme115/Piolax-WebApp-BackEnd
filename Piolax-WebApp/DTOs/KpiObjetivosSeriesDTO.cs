namespace Piolax_WebApp.DTOs
{
    public class KpiObjetivosSeriesDTO
    {
        public int idArea { get; set; }
        public string nombreArea { get; set; }
        public List<KpiSegmentadoDTO> meses { get; set; } = new();
    }
}
