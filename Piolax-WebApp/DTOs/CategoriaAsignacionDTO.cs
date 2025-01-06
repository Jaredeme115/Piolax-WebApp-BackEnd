using System.ComponentModel.DataAnnotations;

namespace Piolax_WebApp.DTOs
{
    public class CategoriaAsignacionDTO
    {
        [Required]
        public string descripcion { get; set; }

        public List<AsignacionesDTO> Asignaciones { get; set; } = new List<AsignacionesDTO>();
    }
}
