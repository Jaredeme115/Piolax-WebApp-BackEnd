namespace Piolax_WebApp.Models
{
    public class EmpleadoAreaRol
    {
        public int idEmpleado { get; set; }
        public Empleado Empleado { get; set; }

        public int idArea { get; set; }
        public Areas Area { get; set; }

        public int idRol { get; set; }
        public Roles Rol { get; set; }
    }
}
