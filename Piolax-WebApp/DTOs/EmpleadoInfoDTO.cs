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
        public int? idArea { get; set; }
        public int? idRol { get; set; }
        public AreaRolDTO areaPrincipal { get; set; }

        // ✅ Nueva propiedad para asignar idArea e idRol automáticamente
        public void SetIdAreaRol()
        {
            if (areaPrincipal != null)
            {
                idArea = areaPrincipal.idArea;
                idRol = areaPrincipal.idRol;
            }
        }

    }
}
