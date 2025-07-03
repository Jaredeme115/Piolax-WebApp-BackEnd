namespace Piolax_WebApp.DTOs
{
    public class ObservacionesMPDTO
    {
        public int idObservacionMP { get; set; }
        public int idHistoricoMP { get; set; }
        public string observacion { get; set; }
        public DateTime? fechaObservacion { get; set; } = DateTime.UtcNow;
    }
}
