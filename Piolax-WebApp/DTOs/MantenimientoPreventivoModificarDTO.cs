using Piolax_WebApp.Models;

namespace Piolax_WebApp.DTOs
{
    public class MantenimientoPreventivoModificarDTO
    {

        public int idFrecuenciaPreventivo { get; set; }

        public string nombreFrecuenciaPreventivo { get; set; }

        public int semanaPreventivo { get; set; }

        public bool Activo { get; set; }

        public int idEmpleado { get; set; }

        public string nombreCompletoTecnicoMP { get; set; }

    }
}
