namespace Piolax_WebApp.DTOs
{
    public class ProyectoEtapaDTO
    {
        public int? Id { get; set; }
        public int ProyectoId { get; set; }
        public byte NivelMR { get; set; }
        public byte Porcentaje { get; set; }
        public DateTime FechaUltActualiza { get; set; }
    }
}
