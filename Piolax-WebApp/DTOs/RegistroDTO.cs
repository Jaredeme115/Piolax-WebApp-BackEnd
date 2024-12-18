using System.ComponentModel.DataAnnotations;

namespace Piolax_WebApp.DTOs
{
    public class RegistroDTO
    {
        [Required(ErrorMessage = "El numero de nomina es requerido")]
        public string numNomina { get; set; }

        [Required(ErrorMessage = "El nombre es requerido")]
        public string nombre { get; set; }

        [Required(ErrorMessage = "El apellido paterno es requerido")]
        public string apellidoPaterno { get; set; }

        [Required(ErrorMessage = "El apellido materno es requerido")]
        public string apellidoMaterno { get; set; }

        [Required(ErrorMessage = "El telefono es requerido")]
        public string telefono { get; set; }

        public string email { get; set; }

        [Required(ErrorMessage = "La fecha de ingreso es requerida")]
        public DateOnly fechaIngreso { get; set; }


        [Required(ErrorMessage = "El password es requerido")]
        public string password { get; set; }

        [Required(ErrorMessage = "El status es requerido")]

        public int idStatusEmpleado { get; set; }

        //Asignación de área y rol (Area y rol principal)

        [Required(ErrorMessage = "El área es requerida")]
        public int idArea { get; set; }

        [Required(ErrorMessage = "El rol es requerido")]
        public int idRol { get; set; }

        //Propiedad que indica si el área es la principal del empleado

        [Required(ErrorMessage = "El área principal es requerida")]
        public bool esAreaPrincipal { get; set; }


    }
}
