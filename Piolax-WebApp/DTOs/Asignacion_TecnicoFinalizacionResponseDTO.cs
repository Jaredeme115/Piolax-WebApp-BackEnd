using Piolax_WebApp.Models;

namespace Piolax_WebApp.DTOs
{
    public class Asignacion_TecnicoFinalizacionResponseDTO
    {
        public int idAsignacionTecnico { get; set; }
        public int idAsignacion { get; set; }
        public int idEmpleado { get; set; }
        public string nombreCompletoTecnico { get; set; } // Nombre completo del técnico
        public DateTime horaInicio { get; set; }
        public DateTime horaTermino { get; set; }
        public string solucion { get; set; }
        public int idStatusAprobacionTecnico { get; set; }
        public string nombreStatusAprobacionTecnico { get; set; } // Descripción del estado de aprobación
        public bool esTecnicoActivo { get; set; }
        public bool paroMaquinaTecnico { get; set; } 

    }
}
