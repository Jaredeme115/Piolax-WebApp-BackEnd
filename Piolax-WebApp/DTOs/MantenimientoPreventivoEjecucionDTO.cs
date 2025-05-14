namespace Piolax_WebApp.DTOs
{
    public class MantenimientoPreventivoEjecucionDTO
    {
        public int idMPEjecucion { get; set; }
        public int idMP { get; set; }
        public string nombrePDF { get; set; } = null!;
        public string rutaPDF { get; set; } = null!;
        public int semanaEjecucion { get; set; }
        public int anioEjecucion { get; set; }
        public DateTime fechaEjecucion { get; set; }
    }
}
