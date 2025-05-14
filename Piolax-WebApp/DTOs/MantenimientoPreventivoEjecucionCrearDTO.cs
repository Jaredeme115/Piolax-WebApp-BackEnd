namespace Piolax_WebApp.DTOs
{
    public class MantenimientoPreventivoEjecucionCrearDTO
    {
        public int idMP { get; set; }
        public string nombrePDF { get; set; } = null!;
        public string rutaPDF { get; set; } = null!;
        public int semanaEjecucion { get; set; }
        public int anioEjecucion { get; set; }
    }
}
