namespace Piolax_WebApp.DTOs
{
    public class PausaAsignacionDTO
    {
        public string comentarioPausa { get; set; }
        public TimeSpan horaTermino { get; set; }
        public int idStatusAprobacionTecnico { get; set; }
    }
}
