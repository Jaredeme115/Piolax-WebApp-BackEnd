namespace Piolax_WebApp.DTOs
{
    public class ProyectoDTO
    {
        public int? Id { get; set; }
        public string ProyectoNo { get; set; } = null!;
        public string NombreProyecto { get; set; } = null!;
        public string? Cliente { get; set; }
        public int? KeyPersonId { get; set; }
        public string StatusProyecto { get; set; } = null!;
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaCierre { get; set; }
    }
}
