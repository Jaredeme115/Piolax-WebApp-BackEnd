using Piolax_WebApp.Models;

namespace Piolax_WebApp.DTOs
{
    public class ObservacionesMPCrearDTO
    {
        public int idMP { get; set; }
        public string observacion { get; set; }
        public DateTime? fechaObservacion { get; set; } = DateTime.UtcNow;
    }
}
