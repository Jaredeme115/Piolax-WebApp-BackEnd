using System.ComponentModel.DataAnnotations;

namespace Piolax_WebApp.DTOs
{
    public class StatusEmpleadoDTO
    {
        [Required(ErrorMessage = "El estatus del empleado es requerido")]
        public string descripcionStatusEmpleado { get; set; }
    }
}
