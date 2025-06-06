namespace Piolax_WebApp.DTOs
{
    public class KpisDetalleDTO
    {
        public int idKPIMantenimiento { get; set; }
        public int idArea { get; set; }
        public DateTime fechaCalculo { get; set; }
        public double MTBF_HorasNueva { get; set; }
    }
}
