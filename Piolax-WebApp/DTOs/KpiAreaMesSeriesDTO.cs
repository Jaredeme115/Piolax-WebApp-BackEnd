namespace Piolax_WebApp.DTOs
{
    public class KpiAreaMesSeriesDTO
    {
        public string nombreArea { get; set; }
        public List<KpiSegmentadoDTO> meses { get; set; } = new();
    }
}
