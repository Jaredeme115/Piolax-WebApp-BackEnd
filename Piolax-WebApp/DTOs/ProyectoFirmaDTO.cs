namespace Piolax_WebApp.DTOs
{
    public class ProyectoFirmaDTO
    {
        public int? Id { get; set; }
        public int ProyectoId { get; set; }
        public int EmpleadoId { get; set; }
        public string NombreCompleto { get; set; } = null!;
        public DateTime FechaFirma { get; set; }

    }
}
