﻿using Piolax_WebApp.Models;
using System.ComponentModel.DataAnnotations;

namespace Piolax_WebApp.DTOs
{
    public class SolicitudesDTO
    {
        [Required]
        public string descripcion { get; set; }

        [Required]
        public DateTime fechaSolicitud { get; set; }

        [Required]
        public int idEmpleado { get; set; }

        [Required]
        public int idMaquina { get; set; }

        [Required]
        public int idTurno { get; set; }

        [Required]
        public int idStatusOrden { get; set; }
        
        [Required]
        public int idStatusAprobacionSolicitante { get; set; }

        [Required]
        public int idArea { get; set; }

        [Required]
        public int idRol { get; set; }




    }
}
