namespace Piolax_WebApp.DTOs
{
    public class EtapaActividadDTO
    {
        public int? Id { get; set; }
        public int EtapaId { get; set; }
        public string Descripcion { get; set; } = null!;
        public int Orden { get; set; }
        public bool Completada { get; set; }
        public DateTime? FechaCompletada { get; set; }
        public int? EmpleadoMarcaId { get; set; }
    }
}
