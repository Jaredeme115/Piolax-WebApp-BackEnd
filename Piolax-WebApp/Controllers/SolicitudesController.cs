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

                await _hubContext.Clients.Group("Mantenimiento").SendAsync("ReceiveRequestDetails", new
                {
                    idSolicitud = solicitudDetalle.idSolicitud,
                    descripcion = solicitudesDTO.descripcion,
                    fechaSolicitud = solicitudesDTO.fechaSolicitud
                });

                return Ok(solicitudDetalle);
            }
            catch (Exception ex)
            {
                // 👇 LOGEA EL ERROR COMPLETO
                Console.WriteLine("❌ Error al registrar la solicitud: " + ex.Message);
                Console.WriteLine("⛔ StackTrace: " + ex.StackTrace);

                return BadRequest(new { message = $"Error al registrar la solicitud: {ex.Message}" });
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
                fechaSolicitud = s.fechaSolicitud,
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
            var solicitudesFormateadas = solicitudes.Select(s => new
            {
                s.idSolicitud,
                s.descripcion,
                fechaSolicitud = s.fechaSolicitud.ToString("dd/MM/yyyy HH:mm:ss"),
                s.nombreCompletoEmpleado,
                s.nombreMaquina,
                s.nombreTurno,
                s.nombreStatusOrden,
                s.nombreStatusAprobacionSolicitante,
                s.area,
                s.rol,
                s.nombreCategoriaTicket,
                s.nombreCompletoTecnico,
                s.solucion,
                // Nuevos campos formateados de horaInicio y horaTermino
                horaInicio = s.horaInicio.HasValue ? s.horaInicio.Value.ToString("dd/MM/yyyy HH:mm:ss") : null,
                horaTermino = s.horaTermino.HasValue ? s.horaTermino.Value.ToString("dd/MM/yyyy HH:mm:ss") : null,
                refacciones = s.Refacciones?.Select(r => new
                {
                    nombreRefaccion = r.NombreRefaccion,
                    cantidad = r.Cantidad
                })
            });

            return Ok(solicitudesFormateadas);
        }

        [HttpGet("ObtenerSolicitudesConPrioridad")]
        public async Task<ActionResult<IEnumerable<SolicitudesDetalleDTO>>> ObtenerSolicitudesConPrioridad()
        {
            var solicitudes = await _service.ObtenerSolicitudesConPrioridadAsync();
            return Ok(solicitudes);
        }

        //[Authorize(Policy = "AdminOnly")]
        [HttpDelete("EliminarSolicitud/{idSolicitud}")]
        public async Task<IActionResult> EliminarSolicitud(int idSolicitud)
        {
            try
            {
                var eliminada = await _service.EliminarSolicitud(idSolicitud);
                if (!eliminada)
                {
                    return NotFound(new { mensaje = "No se encontró la solicitud para eliminar." });
                }

                return Ok(new { mensaje = "Solicitud eliminada exitosamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al eliminar la solicitud.", error = ex.Message });
            }
        }

        [HttpGet("ObtenerSolicitudesPorAreaYRoles/{idArea}")]
        public async Task<ActionResult<IEnumerable<SolicitudesDetalleDTO>>> ObtenerSolicitudesPorAreaYRoles(int idArea)
        {
            // Lista de roles específicos a filtrar (6, 7, 8)
            var rolesEspecificos = new List<int> { 6, 7, 8 };

            var solicitudes = await _service.ObtenerSolicitudesPorAreaYRoles(idArea, rolesEspecificos);

            var solicitudesFormateadas = solicitudes.Select(s => new
            {
                s.idSolicitud,
                s.descripcion,
                fechaSolicitud = s.fechaSolicitud,
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

        [HttpGet("ConsultarSolicitudesTerminadasPorArea/{numNomina}")]
        public async Task<ActionResult<IEnumerable<SolicitudesDetalleDTO>>> ConsultarSolicitudesTerminadasPorArea(string numNomina)
        {
            try
            {
                var solicitudes = await _service.ConsultarSolicitudesTerminadasPorArea(numNomina);

                var solicitudesFormateadas = solicitudes.Select(s => new
                {
                    s.idSolicitud,
                    s.descripcion,
                    fechaSolicitud = s.fechaSolicitud.ToString("dd/MM/yyyy HH:mm:ss"),
                    s.nombreCompletoEmpleado,
                    s.nombreMaquina,
                    s.nombreTurno,
                    s.nombreStatusOrden,
                    s.nombreStatusAprobacionSolicitante,
                    s.area,
                    s.rol,
                    s.nombreCategoriaTicket,
                    s.nombreCompletoTecnico,
                    s.solucion,
                    horaInicio = s.horaInicio.HasValue ? s.horaInicio.Value.ToString("dd/MM/yyyy HH:mm:ss") : null,
                    horaTermino = s.horaTermino.HasValue ? s.horaTermino.Value.ToString("dd/MM/yyyy HH:mm:ss") : null,
                    refacciones = s.Refacciones?.Select(r => new
                    {
                        nombreRefaccion = r.NombreRefaccion,
                        cantidad = r.Cantidad
                    })
                });

                return Ok(solicitudesFormateadas);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al consultar solicitudes terminadas por área: {ex.Message}");
            }
        }

        [HttpGet("ConsultarSolicitudesTerminadasPorEmpleado/{numNomina}")]
        public async Task<ActionResult<IEnumerable<SolicitudesDetalleDTO>>> ConsultarSolicitudesTerminadasPorEmpleado(string numNomina)
        {
            try
            {
                var solicitudes = await _service.ConsultarSolicitudesTerminadasPorEmpleado(numNomina);

                var solicitudesFormateadas = solicitudes.Select(s => new
                {
                    s.idSolicitud,
                    s.descripcion,
                    fechaSolicitud = s.fechaSolicitud.ToString("dd/MM/yyyy HH:mm:ss"),
                    s.nombreCompletoEmpleado,
                    s.nombreMaquina,
                    s.nombreTurno,
                    s.nombreStatusOrden,
                    s.nombreStatusAprobacionSolicitante,
                    s.area,
                    s.rol,
                    s.nombreCategoriaTicket,
                    s.nombreCompletoTecnico,
                    s.solucion,
                    horaInicio = s.horaInicio.HasValue ? s.horaInicio.Value.ToString("dd/MM/yyyy HH:mm:ss") : null,
                    horaTermino = s.horaTermino.HasValue ? s.horaTermino.Value.ToString("dd/MM/yyyy HH:mm:ss") : null,
                    refacciones = s.Refacciones?.Select(r => new
                    {
                        nombreRefaccion = r.NombreRefaccion,
                        cantidad = r.Cantidad
                    })
                });

                return Ok(solicitudesFormateadas);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al consultar solicitudes terminadas del empleado: {ex.Message}");
            }
        }

        [HttpGet("ExportarSolicitudesTerminadasExcel")]
        public async Task<IActionResult> ExportarSolicitudesTerminadasExcel()
        {
            try
            {
                // Llamar al servicio para generar el Excel
                byte[] excelBytes = await _service.ExportarSolicitudesTerminadasExcel();

                // Devolver el archivo para descarga
                return File(
                    excelBytes,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    $"SolicitudesTerminadas_{DateTime.Now:yyyyMMdd}.xlsx");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al generar el archivo Excel: {ex.Message}");
            }
        }

        [HttpGet("ExportarSolicitudesTerminadasPorAreaExcel")]
        public async Task<IActionResult> ExportarSolicitudesTerminadasPorAreaExcel([FromQuery] string numNomina)
        {
            try
            {
                // Llamar al servicio para generar el Excel
                byte[] excelBytes = await _service.ExportarSolicitudesTerminadasPorAreaExcel(numNomina);

                // Devolver el archivo para descarga
                return File(
                    excelBytes,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    $"SolicitudesTerminadas_{DateTime.Now:yyyyMMdd}.xlsx");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al generar el archivo Excel: {ex.Message}");
            }
        }

 
    }
}
