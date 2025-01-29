using System.ComponentModel.DataAnnotations;

namespace Piolax_WebApp.DTOs
{
    public class AsignacionDetallesDTO
    {

        public int idAsignacion { get; set; }

        public int idSolicitud { get; set; }

        public int idStatusAsignacion { get; set; }

        public string nombreStatusAsignacion { get; set; }

        // Información completa de la solicitud asociada
        public SolicitudesDetalleDTO Solicitud { get; set; }

        // Técnicos relacionados con la asignación
        public List<Asignacion_TecnicoDetallesDTO> Tecnicos { get; set; } = new List<Asignacion_TecnicoDetallesDTO>();

        // Refacciones relacionadas con la asignación
        public List<Asignacion_RefaccionesDetallesDTO> Refacciones { get; set; } = new List<Asignacion_RefaccionesDetallesDTO>();
    }
}
