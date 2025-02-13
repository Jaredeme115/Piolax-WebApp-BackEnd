﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Piolax_WebApp.Models
{
    public class Solicitudes
    {
        [Key]
        public int idSolicitud { get; set; }

        [Required]
        public string descripcion { get; set; }

        [Required]
        public DateTime fechaSolicitud { get; set; }

        [Required]
        public int idEmpleado { get; set; }
        public Empleado Empleado { get; set; }

        [Required]
        public int idMaquina { get; set; }
        public Maquinas Maquina { get; set; }

        [Required]
        public int idTurno { get; set; }
        public Turnos Turno { get; set; }

        [Required]
        public int idStatusOrden { get; set; }
        public StatusOrden StatusOrden { get; set; }

        [Required]
        public int idStatusAprobacionSolicitante { get; set; }
        public StatusAprobacionSolicitante StatusAprobacionSolicitante { get; set; }

        [Required]
        public int idAreaSeleccionada { get; set; }

        [Required]
        public int idRolSeleccionado { get; set; }

        [Required]
        public int idCategoriaTicket { get; set; }
        public categoriaTicket categoriaTicket { get; set; }

        //Nuevo atributo agregado para el envio de notificaciones mediante SignalR

        [Required]
        public bool notificado { get; set; } = false;

        public virtual ICollection<Asignaciones> Asignaciones { get; set; } = new List<Asignaciones>(); // Lista de Asignaciones asociados a Solicitud


    }
}
