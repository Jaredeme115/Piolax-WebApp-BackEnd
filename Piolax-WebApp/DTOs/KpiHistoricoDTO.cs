namespace Piolax_WebApp.DTOs
{
    public class KpiHistoricoDTO
    {
        public DateTime fecha { get; set; }
        public List<KpiNombreValorDTO> detalles { get; set; }
    }
}
