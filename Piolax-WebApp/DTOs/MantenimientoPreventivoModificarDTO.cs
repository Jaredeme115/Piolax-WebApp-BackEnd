using Piolax_WebApp.Models;

namespace Piolax_WebApp.DTOs
{
    public class MantenimientoPreventivoModificarDTO
    {

        public int idFrecuenciaPreventivo { get; set; }

        public string nombreFrecuenciaPreventivo { get; set; }

        public int semanaPreventivo { get; set; }

        public bool activo { get; set; }

        public int idEmpleado { get; set; }

        public string nombreCompletoTecnicoMP { get; set; }

        public int semanaOriginalMP { get; set; } // Semana original del mantenimiento preventivo

        public int anioPreventivo { get; set; }

        public int idEstatusPreventivo { get; set; } // este campo debe existir


    }
}
