namespace Piolax_WebApp.Models
{

    public class KpiMantenimiento_Tecnico
    {
        public int idKPIMantenimientoTecnico { get; set; }
        public int idKPIMantenimiento { get; set; }
        public int idEmpleado { get; set; }

        public KpisMantenimiento KpisMantenimiento { get; set; }
        public Empleado Empleado { get; set; }
    }


}
