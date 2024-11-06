using Piolax_WebApp.Models;

namespace Piolax_WebApp.DTOs
{
    public class ResultadoLogin
    {
        public bool esLoginExitoso { get; set; }
        public Empleado empleado { get; set; }
    }
}
