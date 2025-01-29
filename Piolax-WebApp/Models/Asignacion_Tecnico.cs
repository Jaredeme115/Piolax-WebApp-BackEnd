﻿using System.ComponentModel.DataAnnotations;

namespace Piolax_WebApp.Models
{
    public class Asignacion_Tecnico
    {
        [Key]
        public int idAsignacionTecnico { get; set; }

        [Required]
        public int idAsignacion { get; set; }
        public Asignaciones Asignacion { get; set; }

        [Required]
        public int idEmpleado { get; set; }
        public Empleado Empleado { get; set; }

        [Required]
        public int idStatusAprobacionTecnico { get; set; }
        public StatusAprobacionTecnico StatusAprobacionTecnico { get; set; }

        [Required]
        public DateTime horaInicio { get; set; }

        [Required]
        public DateTime horaTermino { get; set; }

        [Required]
        public string solucion { get; set; }

        [Required]
        public string comentarioPausa { get; set; }

        [Required]
        public bool esTecnicoActivo { get; set; }

        public virtual ICollection<asignacion_refacciones> Asignacion_Refacciones { get; set; } = new List<asignacion_refacciones>(); // Lista de asignacion_refacciones asociados a Asignacion_Tecnico
    }
}
