using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Piolax_WebApp.DTOs;
using Piolax_WebApp.Models;
using Piolax_WebApp.Services;
using Piolax_WebApp.Services.Impl;

namespace Piolax_WebApp.Controllers
{
    public class SolicitudesController(ISolicitudService service) : BaseApiController
    {
        private readonly ISolicitudService _service = service;

       
        [HttpPost("Registrar")]
        public async Task<IActionResult> RegistrarSolicitud([FromBody] SolicitudesDTO solicitudesDTO)
        {
            try
            {
                var solicitudDetalle = await _service.RegistrarSolicitud(solicitudesDTO);
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


        [HttpGet("todas")]
        public async Task<ActionResult<IEnumerable<SolicitudesDetalleDTO>>> ObtenerSolicitudes()
        {
            var solicitudes = await _service.ObtenerSolicitudes();
            var solicitudesFormateadas = solicitudes.Select(s => new
            {
                s.idSolicitud,
                s.descripcion,
                fechaSolicitud = s.fechaSolicitud.ToString("dd/MM/yyyy HH:mm:ss"), // Formatear la fecha
                s.nombreCompletoEmpleado,
                s.idMaquina,
                s.idTurno,
                s.idStatusOrden,
                s.idStatusAprobacionSolicitante,
                s.Areas,
                s.Roles,
                s.paroMaquina
            });
            return Ok(solicitudesFormateadas);
        }

    }
 }
