using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;
using Piolax_WebApp.Services;
using Piolax_WebApp.Repositories;
using Piolax_WebApp.Services.Impl;
using Microsoft.AspNetCore.SignalR;
using Piolax_WebApp.Hubs;
using System;
using System.Threading.Tasks;

namespace Piolax_WebApp.Controllers
{
    public class SolicitudesController(ISolicitudService service, IHubContext<NotificationHub> hubContext) : BaseApiController
    {
        private readonly ISolicitudService _service = service;
        private readonly IHubContext<NotificationHub> _hubContext = hubContext;


        [HttpPost("Registrar")]
        public async Task<IActionResult> RegistrarSolicitud([FromBody] SolicitudesDTO solicitudesDTO)
        {
            try
            {
                var solicitudDetalle = await _service.RegistrarSolicitud(solicitudesDTO);

                await _hubContext.Clients.All.SendAsync("ReceiveNewRequest", new
                {
                    idSolicitud = solicitudDetalle.idSolicitud,
                    descripcion = solicitudesDTO.descripcion,
                    fechaSolicitud = solicitudesDTO.fechaSolicitud
                });
                return Ok(solicitudDetalle);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al registrar la solicitud: {ex.Message}");
            }
        }


        [HttpGet("{idSolicitud}/Detalle")]
        public async Task<IActionResult> ObtenerDetalleSolicitud(int idSolicitud)
        {
            var solicitudDetalle = await _service.ObtenerSolicitudConDetalles(idSolicitud);
            if (solicitudDetalle == null)
            {
                return NotFound($"No se encontró la solicitud con ID: {idSolicitud}");
            }

            return Ok(solicitudDetalle);
        }


        [HttpGet("ObtenerSolicitudes")]
        public async Task<ActionResult<IEnumerable<SolicitudesDetalleDTO>>> ObtenerSolicitudes()
        {
            var solicitudes = await _service.ObtenerSolicitudes();
            var solicitudesFormateadas = solicitudes.Select(s => new
            {
                s.idSolicitud,
                s.descripcion,
                fechaSolicitud = s.fechaSolicitud.ToString("dd/MM/yyyy HH:mm:ss"), // Formatear la fecha
                s.nombreCompletoEmpleado,
                s.nombreMaquina,
                s.nombreTurno,
                s.nombreStatusOrden,
                s.nombreStatusAprobacionSolicitante,
                s.area,
                s.rol,
                s.nombreCategoriaTicket
            });
            return Ok(solicitudesFormateadas);
        }

        [HttpGet("ObtenerSolicitudesEmpleado/{numNomina}")]
        public async Task<ActionResult<IEnumerable<SolicitudesDetalleDTO>>> ObtenerSolicitudesEmpleado(string numNomina)
        {
            var solicitudes = await _service.ObtenerSolicitudesEmpleado(numNomina);
            var solicitudesFormateadas = solicitudes.Select(s => new
            {
                s.idSolicitud,
                s.descripcion,
                fechaSolicitud = s.fechaSolicitud.ToString("dd/MM/yyyy HH:mm:ss"), // Formatear la fecha
                s.nombreCompletoEmpleado,
                s.nombreMaquina,
                s.nombreTurno,
                s.nombreStatusOrden,
                s.nombreStatusAprobacionSolicitante,
                s.area,
                s.rol,
                s.nombreCategoriaTicket
            });
            return Ok(solicitudesFormateadas);
        }

        [HttpPut("{idSolicitud}/ModificarEstatusAprobacionSolicitante/{idStatusAprobacionSolicitante}")]
        public async Task<IActionResult> ModificarEstatusAprobacionSolicitante(int idSolicitud, int idStatusAprobacionSolicitante)
        {
            try
            {
                var solicitudDetalle = await _service.ModificarEstatusAprobacionSolicitante(idSolicitud, idStatusAprobacionSolicitante);
                return Ok(solicitudDetalle);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al modificar el estatus de aprobación del solicitante: {ex.Message}");
            }
        }

        [HttpGet("ConsultarSolicitudesNoTomadas")]
        public async Task<ActionResult<IEnumerable<Solicitudes>>> ConsultarSolicitudesNoTomadas()
        {
            var solicitudes = await _service.ConsultarSolicitudesNoTomadas();
            return Ok(solicitudes);

        }

        [HttpGet("ConsultarSolicitudesTerminadas")]
        public async Task<ActionResult<IEnumerable<SolicitudesDetalleDTO>>> ConsultarSolicitudesTerminadas()
        {
            var solicitudes = await _service.ConsultarSolicitudesTerminadas();
            return Ok(solicitudes);

        }

        [HttpGet("ObtenerSolicitudesConPrioridad")]
        public async Task<ActionResult<IEnumerable<SolicitudesDetalleDTO>>> ObtenerSolicitudesConPrioridad()
        {
            var solicitudes = await _service.ObtenerSolicitudesConPrioridadAsync();
            return Ok(solicitudes);
        }


    }
}
