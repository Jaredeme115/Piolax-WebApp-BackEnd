namespace Piolax_WebApp.DTOs
{
    public class PausarAsignacionDTO
    {
        public int idAsignacion { get; set; }
        public int idTecnicoQuePausa { get; set; }
        public string comentarioPausa { get; set; } = string.Empty;
    }
}
