namespace Piolax_WebApp.DTOs
{
    public class EmpleadoAreaRolDTO
    {
        public string numNomina { get; set; }
        public string nombre { get; set; }
        public string apellidoPaterno { get; set; }
        public string apellidoMaterno { get; set; }
        public string telefono { get; set; }
        public string email { get; set; }
        public DateOnly fechaIngreso { get; set; }
        public AreaRolDTO areaPrincipal { get; set; }
        public List<AreaRolDTO> areasSecundarias { get; set; }
    }
}
