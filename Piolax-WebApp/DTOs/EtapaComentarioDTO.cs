namespace Piolax_WebApp.DTOs
{
    public class EtapaComentarioDTO
    {
        public int? Id { get; set; }
        public int EtapaId { get; set; }
        public int EmpleadoId { get; set; }
        public string TextoComentario { get; set; } = null!;
        public DateTime FechaComentario { get; set; }
    }
}
