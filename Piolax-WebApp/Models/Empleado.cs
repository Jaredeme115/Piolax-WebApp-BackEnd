﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Piolax_WebApp.Models
{
    public class Empleado
    {
        [Key]
        public int idEmpleado { get; set; }

        [Required]
        public string numNomina { get; set; }

        [Required]
        public string nombre { get; set; }

        [Required]
        public string apellidoPaterno { get; set; }

        [Required]
        public string apellidoMaterno { get; set; }

        [Required]
        public string telefono { get; set; }

        public string email { get; set; }

        [Required]
        public DateOnly fechaIngreso { get; set; }

        [Required]
        public byte[] passwordHasH { get; set; }

        [Required]
        public byte[] passwordSalt { get; set; }

        // Relación con la tabla StatusEmpleado
        public int idStatusEmpleado { get; set; }
        public StatusEmpleado StatusEmpleado { get; set; } // Propiedad de navegación

    }
}
