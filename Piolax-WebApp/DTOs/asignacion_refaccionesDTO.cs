﻿using System.ComponentModel.DataAnnotations;

namespace Piolax_WebApp.DTOs
{
    public class Asignacion_RefaccionesDTO
    {
        [Required]
        public int idAsignacion { get; set; }

        [Required]
        public int idRefaccion { get; set; }

        [Required]
        public int idAsignacionTecnico { get; set; }

        [Required]
        public int cantidad { get; set; }   
    }
}
