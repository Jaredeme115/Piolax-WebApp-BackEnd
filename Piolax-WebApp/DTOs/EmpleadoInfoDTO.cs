namespace Piolax_WebApp.DTOs
{
    public class EmpleadoInfoDTO
    {
        public string numNomina { get; set; }
        public string nombre { get; set; }
        public string apellidoPaterno { get; set; }
        public string apellidoMaterno { get; set; }
        public string telefono { get; set; }
        public string email { get; set; }
        public DateOnly fechaIngreso { get; set; }
        public int idStatusEmpleado { get; set; }

        public int? idArea { get; set; }  // ✅ Agregar ID del área
        public int? idRol { get; set; }  // ✅ Agregar ID del rol

        public AreaRolDTO areaPrincipal { get; set; }
    }
}
